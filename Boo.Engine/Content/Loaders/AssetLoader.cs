using System.Reflection;
using System.Text;
using Boo.Common;
using Boo.Common.Content;

namespace Boo.Engine.Content;

internal interface LoaderInterface {}

internal abstract class AssetLoader<TypeAsset, TypeAssetData, TypeAssetManifest> : LoaderInterface 
    where TypeAsset : BooAsset
    where TypeAssetData : struct
    where TypeAssetManifest : BaseAssetManifestInfo
{
    public TypeAsset Load(string assetId)
    {
        var data = LoadData(assetId);

        return LoadFromData(data);
    }
    
    public virtual TypeAsset LoadEmbedded(string assetId)
    {
        var path = new StringBuilder();

        path.Append(BooContent.EMBEDDED_ASSETS_NAMESPACE);
        path.Append('.');
        path.Append(BooContent.EmbeddedFolders[typeof(TypeAsset)]);
        path.Append('.');
        path.Append(assetId);
        path.Append(ContentProperties.BinaryExt);

        using var fileStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(path.ToString());

        if (fileStream == null)
        {
            throw new BooException($"Could not load embedded asset: {assetId}");
        }

        var assetData = BooContent.LoadStream<TypeAssetData>(fileStream);

        return LoadFromData(assetData);
    }

    public abstract TypeAsset LoadFromData(TypeAssetData data);

    protected virtual TypeAssetData LoadData(string assetId)
    {
        var assetManifest = BooContent.GetAssetManifestInfo<TypeAsset, TypeAssetManifest>(assetId);
        
        assetManifest.ThrowIfInvalid();

        var imageDataBinPath = AssetsManifestUtils.GetAssetBinaryPath(ContentProperties.AssetsFolder, assetManifest);

        if (!File.Exists(imageDataBinPath)) throw new BooException($"Asset {assetId} is not compiled");
        
        using var binStream = File.OpenRead(imageDataBinPath);
        return BooContent.LoadStream<TypeAssetData>(binStream);
    }
}