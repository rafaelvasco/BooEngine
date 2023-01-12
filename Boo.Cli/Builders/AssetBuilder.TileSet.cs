using Boo.Common.Content;
using Boo.Common.Content.Stb;
using MessagePack;

namespace Boo.Cli.Builders;

public static partial class AssetBuilder
{
    private static string BuildAndExportTileSet(TileSetManifestInfo defaultFileManifest, string assetsFolder)
    {
        defaultFileManifest.ThrowIfInvalid();

        Console.WriteLine($"Building BooTileSet {defaultFileManifest.Id}");

        var tileSetData = BuildTileSetData(defaultFileManifest, assetsFolder);

        var binPath = AssetsManifestUtils.GetAssetBinaryPath(assetsFolder, defaultFileManifest);

        using var stream = File.OpenWrite(binPath);

        MessagePackSerializer.Serialize(stream, tileSetData, ContentProperties.BinarySerializationOptions);
        
        Console.WriteLine($"BooTileSet {defaultFileManifest.Id} built successfully.");

        return binPath;
    }

    private static TileSetData BuildTileSetData(TileSetManifestInfo manifest, string assetsFolder)
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

        var tileSetData = new TileSetData
        {
            ImageData = imageData,
            Id = manifest.Id!,
            TileSize = manifest.TileSize
        };
        
        return tileSetData;
    }
}