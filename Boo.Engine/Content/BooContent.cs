using System.Text.Json;
using Boo.Common;
using Boo.Common.Content;
using Boo.Engine.Graphics;
using Boo.Engine.Toolkit;
using MessagePack;

namespace Boo.Engine.Content;

public static class BooContent
{
    public const string EMBEDDED_ASSETS_NAMESPACE = "Boo.Engine.Content.BaseAssets";
    
    internal static readonly Dictionary<Type, string> EmbeddedFolders = new()
    {
        { typeof(Texture2D), "Textures" },
        { typeof(ShaderProgram), "Shaders" },
        { typeof(BooTileset), "TileSets" },
        { typeof(BooSpriteAtlas), "SpriteAtlases" },
    };

    private static readonly Dictionary<Type, LoaderInterface> _loaders;

    static BooContent()
    {
        _loaders = new Dictionary<Type, LoaderInterface>
        {
            { typeof(Texture2D), new TextureLoader() },
            { typeof(BooTileset), new TileSetLoader() },
            { typeof(BooSpriteAtlas), new SpriteAtlasLoader() },
            { typeof(ShaderProgram), new ShaderLoader() }
        };
    }

    internal static void Init()
    {
        _assets = new Dictionary<string, BooAsset>();

        Manifest = LoadDefinitionObject<AssetsManifest>(ContentProperties.AssetsManifestFile);
    }

    public static T Get<T>(string assetId) where T : BooAsset
    {
        if (_assets == null)
        {
            throw new BooException("Content not initialized");
        }

        if (_assets.TryGetValue(assetId, out var cachedAsset))
        {
            if (cachedAsset is T value)
            {
                return value;
            }
        }

        BooAsset asset = Load<T>(assetId);

        RegisterAsset(assetId, asset);

        return (T)asset;
    }

    public static T GetEmbedded<T>(string assetId) where T : BooAsset
    {
        if (_assets == null)
        {
            throw new BooException("Content not initialized");
        }

        if (_assets.TryGetValue(assetId, out var cachedAsset))
        {
            if (cachedAsset is T value)
            {
                return value;
            }
        }

        BooAsset asset = LoadEmbedded<T>(assetId);

        RegisterAsset(assetId, asset);

        return (T)asset;
    }

    public static void Free(string assetId)
    {
        if (_assets == null)
        {
            throw new BooException($"Content not initialized");
        }

        if (_assets.TryGetValue(assetId, out var asset))
        {
            Free(asset);
        }
    }

    public static T LoadStream<T>(Stream stream)
    {
        var data = MessagePackSerializer.Deserialize<T>(stream, ContentProperties.BinarySerializationOptions);
        
        return data;
    }

    public static BooSettings LoadSettings()
    {
        var path = ContentProperties.GameSettingsFile;

        try
        {
            if (File.Exists(Path.Combine(ContentProperties.AssetsFolder, path)))
            {
                var settingsData = LoadDefinitionObject<BooSettings.BooSettingsData>(path);
                BooSettings settings = new BooSettings(settingsData);
                return settings;
            }
            else
            {
                var settingsData = new BooSettings.BooSettingsData();
                var settings = new BooSettings(settingsData);
                using var stream = File.OpenWrite(path);
                JsonSerializer.Serialize(stream, settingsData, BooSettings.JsonSettings());

                return settings;
            }
        }
        catch (Exception e)
        {
            throw new BooException("Could not load settings.", e);
        }
    }

    internal static LoaderInterface GetLoader<T>() 
        where T : BooAsset
    {
        if (_loaders.TryGetValue(typeof(T), out var loader))
        {
            return loader;
        }

        throw new BooException($"Could not obtain Loader for type {typeof(T)}");
    }
    
    internal static void Free()
    {
        if (_assets == null)
        {
            throw new BooException($"Content not initialized");
        }

        Console.WriteLine("Freeing Game Content...");

        foreach (var (_, asset) in _assets)
        {
            Console.WriteLine($"Freeing asset {asset.Id}...");

            asset.Dispose();
        }

        _assets.Clear();
    }
    
    internal static TypeAssetManifestInfo GetAssetManifestInfo<TypeAsset, TypeAssetManifestInfo>(string assetId) 
        where TypeAsset : BooAsset
        where TypeAssetManifestInfo : BaseAssetManifestInfo
    {
        if (typeof(TypeAsset) == typeof(Texture2D))
        {
            if (Manifest?.Textures != null && Manifest.Textures.TryGetValue(assetId, out var manifestInfo))
            {
                return manifestInfo as TypeAssetManifestInfo ?? throw new InvalidOperationException();
            }
        }

        if (typeof(TypeAsset) == typeof(ShaderProgram))
        {
            if (Manifest?.Shaders != null && Manifest.Shaders.TryGetValue(assetId, out var manifestInfo))
            {
                return manifestInfo as TypeAssetManifestInfo ?? throw new InvalidOperationException();
            }
        }
        
        if (typeof(TypeAsset) == typeof(BooTileset))
        {
            if (Manifest?.TileSets != null && Manifest.TileSets.TryGetValue(assetId, out var manifestInfo))
            {
                return manifestInfo as TypeAssetManifestInfo ?? throw new InvalidOperationException();
            }
        }

        if (typeof(TypeAsset) == typeof(BooSpriteAtlas))
        {
            if (Manifest?.SpriteAtlases != null && Manifest.SpriteAtlases.TryGetValue(assetId, out var manifestInfo))
            {
                return manifestInfo as TypeAssetManifestInfo ?? throw new InvalidOperationException();
            }
        }

        throw new BooException($"Unsupported asset type: {typeof(TypeAsset)}");
    }
    
    private static void Free(BooAsset asset)
    {
        if (_assets == null)
        {
            throw new BooException($"Content not initialized");
        }

        _assets.Remove(asset.Id);
        asset.Dispose();
    }
    
    private static void RegisterAsset(string id, BooAsset asset)
    {
        if (_assets == null)
        {
            throw new BooException($"Content not initialized");
        }

        _assets.Add(id, asset);
    }

    /// <summary>
    /// Deserializes a JSON file to an object of given generic type
    /// </summary>
    /// <param name="fileName">Filename without extension</param>
    /// <param name="options">Option serialization options</param>
    /// <typeparam name="T">Object type</typeparam>
    /// <returns></returns>
    public static T LoadDefinitionObject<T>(string fileName, JsonSerializerOptions? options = null)
    {
        options ??= BooSettings.JsonSettings();        
        
        try
        {
            if (!fileName.Contains(".json"))
            {
                fileName += ".json";
            }
            
            var jsonFile = File.ReadAllText(Path.Combine(ContentProperties.AssetsFolder, fileName));
            
            T? definitionObject = JsonSerializer.Deserialize<T>(jsonFile, options);

            return definitionObject!;
        }
        catch (Exception e)
        {
            throw new BooException($"Failed to load definition file of type {typeof(T)}", e);
        }
    }

    private static T Load<T>(string assetId) where T : BooAsset
    {
        T? asset;

        if (assetId == null)
        {
            throw new ArgumentNullException(nameof(assetId));
        }

        LoaderInterface loader = GetLoader<T>();

        if (typeof(T) == typeof(Texture2D))
        {
            asset = ((loader as TextureLoader)!).Load(assetId) as T;
        }
        else if (typeof(T) == typeof(ShaderProgram))
        {
            asset = ((loader as ShaderLoader)!).Load(assetId) as T;
        }
        else if (typeof(T) == typeof(BooTileset))
        {
            asset = ((loader as TileSetLoader)!).Load(assetId) as T;
        }
        else if (typeof(T) == typeof(BooSpriteAtlas))
        {
            asset = ((loader as SpriteAtlasLoader)!).Load(assetId) as T;
        }
        else
        {
            throw new BooException($"Invalid asset type");
        }

        if (asset == null)
        {
            throw new BooException($"Failed to load asset. Asset is null.");
        }

        return asset;
    }

    private static T LoadEmbedded<T>(string assetId) where T : BooAsset
    {
        T? asset;
        
        LoaderInterface loader = GetLoader<T>();

        if (typeof(T) == typeof(Texture2D))
        {
            asset = ((loader as TextureLoader)!).LoadEmbedded(assetId) as T;
        }
        else if (typeof(T) == typeof(ShaderProgram))
        {
            asset = ((loader as ShaderLoader)!).LoadEmbedded(assetId) as T;
        }
        else if (typeof(T) == typeof(BooTileset))
        {
            asset = ((loader as TileSetLoader)!).LoadEmbedded(assetId) as T;
        }
        else if (typeof(T) == typeof(BooSpriteAtlas))
        {
            asset = ((loader as SpriteAtlasLoader)!).LoadEmbedded(assetId) as T;
        }
        else
        {
            throw new BooException($"Invalid asset type");
        }

        return asset!;
    }
    
    private static Dictionary<string, BooAsset>? _assets;

    private static AssetsManifest? Manifest { get; set; }
    
}