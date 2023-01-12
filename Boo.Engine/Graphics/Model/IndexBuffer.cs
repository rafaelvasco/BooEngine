using Boo.Native;

namespace Boo.Engine.Graphics;

public sealed unsafe class IndexBuffer : BooDisposable
{
    public int IndexCount { get; }

    public IndexBuffer(Memory<short> indices)
    {
        Handle = Bgfx.create_index_buffer(BgfxUtils.MakeRef(indices), (ushort)Bgfx.BufferFlags.None);

        IndexCount = indices.Length;
        
        BooGraphics.RegisterRenderResource(this);
    }

    protected override void Free()
    {
        if (Handle.Valid)
        {
            Bgfx.destroy_index_buffer(Handle);    
        }
    }

    internal Bgfx.IndexBufferHandle Handle { get; }
}