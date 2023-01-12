using System.Text.Json;
using Boo.Common;
using Boo.Common.Content;
using Boo.Common.Graphics;

namespace Boo.Cli.Builders;

public static partial class AssetBuilder
{
    private static AssetsManifest LoadAssetsManifest(string rootPath)
    {
        try
        {
            var jsonFile = File.ReadAllText(Path.Combine(rootPath, ContentProperties.AssetsManifestFile));

            AssetsManifest? manifest = JsonSerializer.Deserialize<AssetsManifest>(jsonFile);

            return manifest ?? throw new BooException($"Failed to load assets Manifest: Object is null.");
        }
        catch (Exception e)
        {
            throw new BooException($"Could not load assets manifest file.", e);
        }
    }

    public static void BuildAssets(string assetsFolder)
    {
        var builtBinariesPaths = new List<string>();

        void Cleanup()
        {
            foreach (var path in builtBinariesPaths)
            {
                File.Delete(path);
            }
        }

        Console.WriteLine($"Building assets on folder {assetsFolder}");

        var manifest = LoadAssetsManifest(assetsFolder);

        if (manifest.Textures != null)
        {
            foreach (var (_, imageManifest) in manifest.Textures)
            {
                try
                {
                    var exportedBinPath = BuildAndExportImage(imageManifest, assetsFolder);
                    builtBinariesPaths.Add(exportedBinPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to build texture {imageManifest.Id}: {e.Message}");
                    Cleanup();
                    return;
                }
            }
        }

        if (manifest.Shaders != null)
        {
            foreach (var (_, shaderManifest) in manifest.Shaders)
            {
                try
                {
                    var exportedBinShaderD3DPath =
                        BuildAndExportShader(shaderManifest, assetsFolder, GraphicsApi.Direct3D11);
                    var exportedBinShaderGlPath =
                        BuildAndExportShader(shaderManifest, assetsFolder, GraphicsApi.OpenGl);

                    builtBinariesPaths.Add(exportedBinShaderD3DPath);
                    builtBinariesPaths.Add(exportedBinShaderGlPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to build shader {shaderManifest.Id}: {e.Message}");
                    Cleanup();
                    return;
                }
            }
        }

        if (manifest.TileSets != null)
        {
            foreach (var (_, tileSetManifest) in manifest.TileSets)
            {
                try
                {
                    var exportedBinPath = BuildAndExportTileSet(tileSetManifest, assetsFolder);
                    builtBinariesPaths.Add(exportedBinPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to build BooTileSet {tileSetManifest.Id}: {e.Message}");
                    Cleanup();
                    return;
                }
            }
        }
        
        if (manifest.SpriteAtlases != null)
        {
            foreach (var (_, atlasManifest) in manifest.SpriteAtlases)
            {
                try
                {
                    var exportedBinPath = BuildAndExportSpriteAtlas(atlasManifest, assetsFolder);
                    builtBinariesPaths.Add(exportedBinPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to build BooSpriteAtlas {atlasManifest.Id}: {e.Message}");
                    Cleanup();
                    return;
                }
            }
        }

        Console.WriteLine(!manifest.IsEmpty ? "All assets built successfully!" : "Nothing to build, exiting.");
    }
}