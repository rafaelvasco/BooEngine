using Boo.Native;

namespace Boo.Engine.Graphics;


internal struct RenderTargetCreateInfo : IBooCreateInfo
{
    public int Width = 0;
    public int Height = 0;
    public const Bgfx.TextureFormat Format = Bgfx.TextureFormat.BGRA8;

    public const Bgfx.SamplerFlags Flags = Bgfx.SamplerFlags.UClamp | Bgfx.SamplerFlags.VClamp |
                                           Bgfx.SamplerFlags.MinPoint | Bgfx.SamplerFlags.MagPoint;

    public RenderTargetCreateInfo()
    {
    }

    public bool IsValid()
    {
        return Width > 0 || Height > 0;
    }
}

public class RenderTarget : BooDisposable
{
    public Texture2D Texture => _texture;

    public int Width => _texture.Width;

    public int Height => _texture.Height;

    internal RenderTarget(Bgfx.FrameBufferHandle handle, int width, int height, Bgfx.SamplerFlags flags)
    {
        Handle = handle;
        _texture = new Texture2D("renderTargerTex", BooGraphics.GetFrameBufferTexture(handle, 0), width, height, flags);
    }
    
    protected override void Free()
    {
    }

    internal readonly Bgfx.FrameBufferHandle Handle;

    private readonly Texture2D _texture;
}