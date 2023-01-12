using System.Text.Json.Serialization;

namespace Boo.Common.Graphics;

public enum GraphicsApi
{
    [JsonPropertyName("direct3d11")] Direct3D11,
    [JsonPropertyName("direct3d12")] Direct3D12,
    [JsonPropertyName("vulkan")] Vulkan,
    [JsonPropertyName("metal")] Metal,
    [JsonPropertyName("opengl")] OpenGl,
    [JsonPropertyName("auto")] Auto
}