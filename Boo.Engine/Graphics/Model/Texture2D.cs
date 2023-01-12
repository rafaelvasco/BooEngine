using Boo.Engine.Content;
using Boo.Native;

namespace Boo.Engine.Graphics;

internal struct Texture2DCreateInfo : IBooCreateInfo
{
    public Memory<byte> Data = Memory<byte>.Empty;
    public int Width = 0;
    public int Height = 0;
    public int BytesPerPixel = 4;
    public const Bgfx.TextureFormat Format = Bgfx.TextureFormat.BGRA8;

    public const Bgfx.SamplerFlags Flags = Bgfx.SamplerFlags.UClamp | Bgfx.SamplerFlags.VClamp |
                                           Bgfx.SamplerFlags.MinPoint | Bgfx.SamplerFlags.MagPoint;

    public Texture2DCreateInfo()
    {
    }

    public bool IsValid()
    {
        return Width > 0 || Height > 0 || !Data.IsEmpty;
    }
}

public enum TextureFilter
{
    NearestNeighbor,
    Linear
}

public class Texture2D : BooAsset, IEquatable<Texture2D>
{
    public readonly int Width;

    public readonly int Height;

    public bool Tiled
    {
        get => (Flags & Bgfx.SamplerFlags.UClamp) == 0 && (Flags & Bgfx.SamplerFlags.VClamp) == 0;
        set => Flags = CalculateFlags(value, Filter);
    }

    public TextureFilter Filter
    {
        get => (Flags & Bgfx.SamplerFlags.Point) != 0
            ? TextureFilter.NearestNeighbor
            : TextureFilter.Linear;

        set => Flags = CalculateFlags(Tiled, value);
    }

    internal Bgfx.SamplerFlags Flags { get; private set; }

    internal Bgfx.TextureHandle Handle { get; }

    internal Texture2D(string id, Bgfx.TextureHandle handle, int width, int height, Bgfx.SamplerFlags flags) : base(id)
    {
        Handle = handle;
        Flags = flags;
        Width = width;
        Height = height;
    }

    ~Texture2D()
    {
        Dispose();
    }

    public unsafe void SetData(Memory<byte> pixels, int targetX = 0, int targetY = 0, int targetW = 0, int targetH = 0)
    {
        var data = BgfxUtils.MakeRef(pixels);

        if (targetW == 0)
        {
            targetW = Width;
        }

        if (targetH == 0)
        {
            targetH = Height;
        }

        Bgfx.update_texture_2d(Handle, 0, 0, (ushort)targetX, (ushort)targetY, (ushort)targetW, (ushort)targetH, data,
            ushort.MaxValue);
    }

    protected override void Free()
    {
        Bgfx.destroy_texture(Handle);
    }

    private static Bgfx.SamplerFlags CalculateFlags(bool tiled, TextureFilter filter)
    {
        var flags = Bgfx.SamplerFlags.None;

        if (!tiled)
        {
            flags = Bgfx.SamplerFlags.UClamp | Bgfx.SamplerFlags.VClamp;
        }

        if (filter == TextureFilter.NearestNeighbor)
        {
            flags |= Bgfx.SamplerFlags.Point;
        }

        return flags;
    }

    public bool Equals(Texture2D? other)
    {
        return other != null && Handle.idx == other.Handle.idx;
    }

    public override bool Equals(object? obj)
    {
        return obj != null && Equals(obj as Texture2D);
    }

    public override int GetHashCode()
    {
        return Handle.idx.GetHashCode();
    }
}