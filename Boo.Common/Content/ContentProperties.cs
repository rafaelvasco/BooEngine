using Boo.Common.Graphics;
using MessagePack;

namespace Boo.Common.Content;

public static class ContentProperties
{
    public const string AssetsFolder = "Content";
    public const string GameSettingsFile = "settings.json";
    public const string AssetsManifestFile = "assets.json";
    public const string BinaryExt = ".bnb";
    
    public static readonly MessagePackSerializerOptions BinarySerializationOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
    
    public static readonly Dictionary<GraphicsApi, string> ShaderAppendStrings =
        new()
        {
            {
                GraphicsApi.Direct3D11, "_D3D"
            },
            {
                GraphicsApi.Direct3D12, "_D3D12"
            },
            {
                GraphicsApi.OpenGl, "_GL"
            },
            {
                GraphicsApi.Metal, "_MT"
            },
            {
                GraphicsApi.Vulkan, "_VK"
            },
        };

}