using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Boo.Common;
using Boo.Common.Graphics;

namespace Boo.Cli.Builders.Compilation;

internal static class ShaderCompiler
{
    private const string COMPILER_PATH = "ShaderCompiler/shaderc.exe";
    private const string INCLUDE_PATH = "ShaderCompiler";
    private const string SAMPLER_REGEX_VAR = "sampler";
    private const string SAMPLER_REGEX = @"SAMPLER2D\s*\(\s*(?<sampler>\w+)\s*\,\s*(?<index>\d+)\s*\)\s*\;";
    private const string PARAM_REGEX_VAR = "param";
    private const string VEC_PARAM_REGEX = @"uniform\s+vec4\s+(?<param>\w+)\s*\;";

    private const string D3D_COMPILE_PARAMS =
        "--platform windows -p $profile_5_0 -O 3 --type $type -f $path -o $output -i $include";

    private const string GLSL_COMPILE_PARAMS =
        "--platform linux --type $type -f $path -o $output -i $include";

    public static ShaderCompileResult Compile(string vsSrcPath, string fsSrcPath, GraphicsApi graphicsApi)
    {
        var tempVsBinOutput = string.Empty;
        var tempFsBinOutput = string.Empty;

        var vsBuildResult = string.Empty;
        var fsBuildResult = string.Empty;

        var processInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = COMPILER_PATH
        };

        var compileParams = graphicsApi switch
        {
            GraphicsApi.Direct3D11 => D3D_COMPILE_PARAMS,
            GraphicsApi.OpenGl => GLSL_COMPILE_PARAMS,
            _ => throw new BooException($"Unsupported GraphicsApi for shader compilation: {graphicsApi}")
        };


        var vsArgs = new StringBuilder(compileParams);

        vsArgs.Replace("$path", vsSrcPath);
        vsArgs.Replace("$type", "vertex");

        if (graphicsApi == GraphicsApi.Direct3D11)
        {
            vsArgs.Replace("$profile", "vs");
        }

        tempVsBinOutput = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(vsSrcPath) + ".bin");

        vsArgs.Replace("$output", tempVsBinOutput);
        vsArgs.Replace("$include", INCLUDE_PATH);

        processInfo.Arguments = vsArgs.ToString();


        var procVs = Process.Start(processInfo);

        procVs?.WaitForExit();

        var outputVs = procVs?.ExitCode ?? -1;

        if (outputVs != 0 && outputVs != -1)
        {
            using var reader = procVs?.StandardOutput;
            vsBuildResult = reader?.ReadToEnd();
        }

        var fsArgs = new StringBuilder(compileParams);

        fsArgs.Replace("$path", fsSrcPath);
        fsArgs.Replace("$type", "fragment");

        if (graphicsApi == GraphicsApi.Direct3D11)
        {
            fsArgs.Replace("$profile", "ps");
        }

        tempFsBinOutput = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(fsSrcPath) + ".bin");

        fsArgs.Replace("$output", tempFsBinOutput);

        fsArgs.Replace("$include", INCLUDE_PATH);

        processInfo.Arguments = fsArgs.ToString();

        var procFs = Process.Start(processInfo);

        procFs?.WaitForExit();

        var outputFs = procFs?.ExitCode ?? -1;

        if (outputFs != 0 && outputFs != -1)
        {
            using var reader = procFs?.StandardOutput;
            fsBuildResult = reader?.ReadToEnd();
        }


        bool vsBinGenerated = File.Exists(tempVsBinOutput);
        bool fsBinGenerated = File.Exists(tempFsBinOutput);

        if (vsBinGenerated && fsBinGenerated)
        {
            var vsBytes = File.ReadAllBytes(tempVsBinOutput);
            var fsBytes = File.ReadAllBytes(tempFsBinOutput);

            var fsStream = File.OpenRead(fsSrcPath);

            ParseUniforms(fsStream, out var samplers, out var @params);

            var result = new ShaderCompileResult(
                vsBytes,
                fsBytes,
                samplers,
                @params
            );

            File.Delete(tempVsBinOutput);
            File.Delete(tempFsBinOutput);

            return result;
        }

        if (vsBinGenerated)
        {
            File.Delete(tempVsBinOutput);
        }

        if (fsBinGenerated)
        {
            File.Delete(tempFsBinOutput);
        }

        throw new BooException(
            $"Shader Compilation Error on {vsSrcPath} and {fsSrcPath}: VSResult: {vsBuildResult}, FSResult: {fsBuildResult}");
    }

    private static void ParseUniforms(FileStream fsStream, out string[] samplers, out string[] @params)
    {
        var samplerRegex = new Regex(SAMPLER_REGEX);
        var paramRegex = new Regex(VEC_PARAM_REGEX);

        var samplersList = new List<string>();
        var paramsList = new List<string>();

        using var reader = new StreamReader(fsStream);

        while (reader.ReadLine() is { } line)
        {
            Match samplerMatch = samplerRegex.Match(line);

            if (samplerMatch.Success)
            {
                string samplerName = samplerMatch.Groups[SAMPLER_REGEX_VAR].Value;
                samplersList.Add(samplerName);
            }
            else
            {
                Match paramMatch = paramRegex.Match(line);

                if (!paramMatch.Success) continue;

                string paramName = paramMatch.Groups[PARAM_REGEX_VAR].Value;

                paramsList.Add(paramName);
            }
        }

        samplers = samplersList.Count > 0 ? samplersList.ToArray() : Array.Empty<string>();

        @params = paramsList.Count > 0 ? paramsList.ToArray() : Array.Empty<string>();
    }
}