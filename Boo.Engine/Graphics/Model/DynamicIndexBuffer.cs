using Boo.Native;

namespace Boo.Engine.Graphics;

public unsafe class DynamicIndexBuffer : BooDisposable
{
    public int IndexCount { get; private set; }

    public DynamicIndexBuffer(Memory<short>? indices = null)
    {
        if (indices.HasValue)
        {
            Handle = Bgfx.create_dynamic_index_buffer_mem(
                BgfxUtils.MakeRef(indices.Value),
                (ushort)Bgfx.BufferFlags.AllowResize
            );
            
            IndexCount = indices.Value.Length;
        }
        else
        {
            Handle = Bgfx.create_dynamic_index_buffer(
                0,
                (ushort)Bgfx.BufferFlags.AllowResize
            );

            IndexCount = 0;
        }
        
        BooGraphics.RegisterRenderResource(this);
    }

    public void SetData(short[] data, int startIndex = 0)
    {
        Bgfx.update_dynamic_index_buffer(
            Handle,
            (uint)startIndex,
            BgfxUtils.MakeRef(data)
        );

        IndexCount = data.Length;
    }

    protected override void Free()
    {
        if (Handle.Valid)
        {
            Bgfx.destroy_dynamic_index_buffer(Handle);
        }
    }


    internal Bgfx.DynamicIndexBufferHandle Handle { get; }
}