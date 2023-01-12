using Boo.Native;

namespace Boo.Engine.Graphics;

public unsafe struct TransientIndexBuffer
{
    internal readonly Bgfx.TransientIndexBuffer Handle;

    internal TransientIndexBuffer(Span<short> indices)
    {
        var handle = new Bgfx.TransientIndexBuffer();
        
        Bgfx.alloc_transient_index_buffer(&handle, (uint)indices.Length, false);

        Handle = handle;
    }
}