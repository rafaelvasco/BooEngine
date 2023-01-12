using Boo.Common.Content;
using Boo.Common.Content.Stb;
using MessagePack;

namespace Boo.Cli.Builders;

public static partial class AssetBuilder
{
    private static string BuildAndExportImage(DefaultFileManifestInfo defaultFileManifest, string assetsFolder)
    {
        defaultFileManifest.ThrowIfInvalid();

        Console.WriteLine($"Building texture {defaultFileManifest.Id}");

        var imageData = BuildImageData(defaultFileManifest, assetsFolder);

        var binPath = AssetsManifestUtils.GetAssetBinaryPath(assetsFolder, defaultFileManifest);

        using var stream = File.OpenWrite(binPath);

        MessagePackSerializer.Serialize(stream, imageData, ContentProperties.BinarySerializationOptions);
        
        Console.WriteLine($"Texture {defaultFileManifest.Id} built successfully.");

        return binPath;
    }

    private static ImageData BuildImageData(DefaultFileManifestInfo defaultFileManifest, string assetsFolder)
    {
        using var stream = File.OpenRead(Path.Combine(assetsFolder, defaultFileManifest.Path!));

        var stbImage = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        var result = new ImageData
        {
            Id = defaultFileManifest.Id!,
            Data = stbImage.Data,
            Width = stbImage.Width,
            Height = stbImage.Height,
            BytesPerPixel = 4
        };
        
        return result;
    }
}