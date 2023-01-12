using MessagePack;

namespace Boo.Common.Content;

[MessagePackObject]
public readonly struct ImageData
{
    [Key(0)]
    public string Id { get; init; }

    [Key(1)]
    public byte[] Data { get; init; }

    [Key(2)]
    public int BytesPerPixel { get; init; }

    [Key(3)]
    public int Width { get; init; }

    [Key(4)]
    public int Height { get; init; }
}