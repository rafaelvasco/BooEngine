using Boo.Common.Content;
using Boo.Common.Content.Stb;
using MessagePack;

namespace Boo.Cli.Builders;

public static partial class AssetBuilder
{
    private static string BuildAndExportSpriteAtlas(SpriteAtlasManifestInfo manifest, string assetsFolder)
    {
        manifest.ThrowIfInvalid();

        Console.WriteLine($"Building BooSpriteAtlas {manifest.Id}");

        var spriteAtlasData = BuildAtlasData(manifest, assetsFolder);

        var binPath = AssetsManifestUtils.GetAssetBinaryPath(assetsFolder, manifest);

        using var stream = File.OpenWrite(binPath);

        MessagePackSerializer.Serialize(stream, spriteAtlasData, ContentProperties.BinarySerializationOptions);
        
        Console.WriteLine($"BooSpriteAtlas {manifest.Id} built successfully.");

        return binPath;
    }

    private static SpriteAtlasData BuildAtlasData(SpriteAtlasManifestInfo manifest, string assetsFolder)
    {
        using var stream = File.OpenRead(Path.Combine(assetsFolder, manifest.Path!));

        var stbImage = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        var imageData = new ImageData
        {
            Id = $"{manifest.Id}_ImageData",
            Data = stbImage.Data,
            Width = stbImage.Width,
            Height = stbImage.Height,
            BytesPerPixel = 4
        };

        var spriteAtlasData = new SpriteAtlasData()
        {
            ImageData = imageData,
            Id = manifest.Id!,
            Regions = manifest.Regions
        };
        
        return spriteAtlasData;
    }
}