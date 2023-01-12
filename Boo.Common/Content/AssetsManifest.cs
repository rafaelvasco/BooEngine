using System.Text;
using System.Text.Json.Serialization;
using Boo.Common.Math;

namespace Boo.Common.Content;

public abstract class BaseAssetManifestInfo
{
    [JsonPropertyName("id")] public string? Id { get; init; }

    protected abstract bool IsValid { get; }

    public void ThrowIfInvalid()
    {
        if (!IsValid)
        {
            throw new BooException($"Invalid Manifest");
        }
    }
}

public class DefaultFileManifestInfo : BaseAssetManifestInfo
{
    [JsonPropertyName("path")] public string? Path { get; init; }

    protected override bool IsValid => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Path);
}

public class ShaderManifestInfo : BaseAssetManifestInfo
{
    [JsonPropertyName("vsPath")] public string? VsPath { get; init; }

    [JsonPropertyName("fsPath")] public string? FsPath { get; init; }

    protected override bool IsValid =>
        !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(VsPath) && !string.IsNullOrEmpty(FsPath);
}

public class TileSetManifestInfo : DefaultFileManifestInfo
{
    [JsonPropertyName("tileSize")]
    public int TileSize { get; init; }
}

public class SpriteAtlasManifestInfo : DefaultFileManifestInfo
{
    [JsonPropertyName("regions")]
    public Dictionary<string, Rect> Regions { get; init; }
}

public class AssetsManifest
{
    [JsonPropertyName("textures")] public Dictionary<string, DefaultFileManifestInfo>? Textures { get; init; }

    [JsonPropertyName("tileSets")] public Dictionary<string, TileSetManifestInfo>? TileSets { get; init; }
    
    [JsonPropertyName("spriteAtlases")] public Dictionary<string, SpriteAtlasManifestInfo>? SpriteAtlases { get; init; }
    
    [JsonPropertyName("shaders")] public Dictionary<string, ShaderManifestInfo>? Shaders { get; init; }

    public bool IsEmpty => Textures == null && Shaders == null && TileSets == null && SpriteAtlases == null;
}

public static class AssetsManifestUtils
{
    public static string GetAssetBinaryPath<T>(string assetsFolder, T assetManifest, string? fileNameAppend = null)
        where T : BaseAssetManifestInfo
    {
        var fileName = new StringBuilder(assetManifest.Id);

        if (fileNameAppend != null)
        {
            fileName.Append($"{fileNameAppend}");
        }

        fileName.Append(ContentProperties.BinaryExt);

        if (typeof(T) == typeof(DefaultFileManifestInfo) || typeof(T).IsSubclassOf(typeof(DefaultFileManifestInfo)))
        {
            return Path.Combine(assetsFolder, Path.GetDirectoryName(((assetManifest as DefaultFileManifestInfo)!).Path)!,
                fileName.ToString());
        }

        if (typeof(T) == typeof(ShaderManifestInfo))
        {
            return Path.Combine(assetsFolder, Path.GetDirectoryName(((assetManifest as ShaderManifestInfo)!).VsPath)!,
                fileName.ToString());
        }

        // if (typeof(T) == typeof(FontManifestInfo))
        // {
        //     return Path.Combine(assetsFolder, Path.GetDirectoryName(((assetManifest as FontManifestInfo)!).Path)!,
        //         fileName.ToString());
        // }

        throw new ArgumentException("Invalid passed type: ", nameof(T));
    }
}