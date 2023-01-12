using System.Reflection;
using System.Text;
using Boo.Common;
using Boo.Common.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Content;

internal class ShaderLoader : AssetLoader<ShaderProgram, ShaderData, ShaderManifestInfo>
{
    public override ShaderProgram LoadFromData(ShaderData data)
    {
        var shader = BooGraphics.CreateShaderProgram(data.Id, new ShaderProgramCreateInfo()
        {
            VertexShader = data.VertexShader,
            FragmentShader = data.FragmentShader,
            Parameters = data.Params,
            Samplers = data.Samplers
        });

        return shader;
    }

    protected override ShaderData LoadData(string assetId)
    {
        var assetManifest = BooContent.GetAssetManifestInfo<ShaderProgram, ShaderManifestInfo>(assetId);

        var shaderBinPath = AssetsManifestUtils.GetAssetBinaryPath(ContentProperties.AssetsFolder, assetManifest,
            ContentProperties.ShaderAppendStrings[BooGraphics.GraphicsApi]);

        if (!File.Exists(shaderBinPath)) throw new BooException($"Asset {assetId} is not compiled.");
        
        using var stream = File.OpenRead(shaderBinPath);
        var data = BooContent.LoadStream<ShaderData>(stream);
        return data;
    }

    public override ShaderProgram LoadEmbedded(string assetId)
    {
        var path = new StringBuilder();

        path.Append(BooContent.EMBEDDED_ASSETS_NAMESPACE);
        path.Append(".Shaders.");
        path.Append(assetId);
        path.Append(ContentProperties.ShaderAppendStrings[BooGraphics.GraphicsApi]);
        path.Append(ContentProperties.BinaryExt);

        using var fileStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(path.ToString());

        if (fileStream == null)
        {
            throw new BooException($"Could not load embedded asset: {assetId}");
        }

        var shaderData = BooContent.LoadStream<ShaderData>(fileStream);

        return LoadFromData(shaderData);
    }
}