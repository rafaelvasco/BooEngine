using Boo.Cli.Builders.Compilation;
using Boo.Common.Content;
using Boo.Common.Graphics;
using MessagePack;

namespace Boo.Cli.Builders;

public static partial class AssetBuilder
{
    private static string BuildAndExportShader(ShaderManifestInfo shaderManifest, string assetsFolder,
        GraphicsApi graphicsApi)
    {
        shaderManifest.ThrowIfInvalid();
        
        Console.WriteLine($"Building shader {shaderManifest.Id} for graphics api {graphicsApi}...");

        var shaderData = BuildShaderData(shaderManifest, assetsFolder, graphicsApi);

        var binPath = AssetsManifestUtils.GetAssetBinaryPath(assetsFolder, shaderManifest,
            fileNameAppend: ContentProperties.ShaderAppendStrings[graphicsApi]);
        
        using var stream = File.OpenWrite(binPath);

        MessagePackSerializer.Serialize(stream, shaderData, ContentProperties.BinarySerializationOptions);

        Console.WriteLine($"Shader {shaderManifest.Id} for graphics api {graphicsApi} built successfully.");

        return binPath;
    }

    private static ShaderData BuildShaderData(ShaderManifestInfo shaderManifest, string assetsFolder,
        GraphicsApi graphicsApi)
    {
        var vsFullPath = Path.Combine(assetsFolder, shaderManifest.VsPath!);
        var fsFullPath = Path.Combine(assetsFolder, shaderManifest.FsPath!);

        var compileResult = ShaderCompiler.Compile(vsFullPath, fsFullPath, graphicsApi);

        var data = new ShaderData
        {
            Id = shaderManifest.Id!,
            VertexShader = compileResult.VsBytes,
            FragmentShader = compileResult.FsBytes,
            Params = compileResult.Params,
            Samplers = compileResult.Samplers
        };

        return data;
    }
}