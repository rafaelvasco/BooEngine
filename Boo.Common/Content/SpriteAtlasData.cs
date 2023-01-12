using Boo.Common.Math;
using MessagePack;

namespace Boo.Common.Content;

[MessagePackObject]
public readonly struct SpriteAtlasData
{
    [Key(0)]
    public string Id { get; init; }

    [Key(1)]
    public ImageData ImageData { get; init; }
    
    [Key(2)]
    public Dictionary<string, Rect> Regions { get; init; }
}