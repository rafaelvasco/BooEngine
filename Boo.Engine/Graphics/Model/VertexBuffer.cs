using Boo.Native;

namespace Boo.Engine.Graphics;

public sealed unsafe class VertexBuffer: BooDisposable
{
    public readonly int VertexCount;

    public VertexBuffer(VertexLayout layout, Memory<Vertex> vertices)
    {
        Handle = Bgfx.create_vertex_buffer(
            BgfxUtils.MakeRef(vertices),
            layout.LayoutData,
            (ushort)Bgfx.BufferFlags.None
        );

        VertexCount = vertices.Length;
        
        BooGraphics.RegisterRenderResource(this);
    }

    protected override void Free()
    {
        if (Handle.Valid)
        {
            Bgfx.destroy_vertex_buffer(Handle);    
        }
    }

    internal Bgfx.VertexBufferHandle Handle { get; }
}
