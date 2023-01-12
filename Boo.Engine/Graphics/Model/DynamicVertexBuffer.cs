using Boo.Native;

namespace Boo.Engine.Graphics;

public unsafe class DynamicVertexBuffer: BooDisposable
{
    public int VertexCount { get; private set; }

    public DynamicVertexBuffer(VertexLayout layout, Memory<Vertex>? vertices = null)
    {
        if (vertices.HasValue)
        {
            Handle = Bgfx.create_dynamic_vertex_buffer_mem(
                BgfxUtils.MakeRef(vertices.Value),
                layout.LayoutData,
                (ushort)Bgfx.BufferFlags.AllowResize
            );

            VertexCount = vertices.Value.Length;
        }
        else
        {
            Handle = Bgfx.create_dynamic_vertex_buffer(
                0,
                layout.LayoutData,
                (ushort)Bgfx.BufferFlags.AllowResize
            );

            VertexCount = 0;
        }
        
        BooGraphics.RegisterRenderResource(this);
    }

    public void SetData(Vertex[] data, int startVertex = 0)
    {
        Bgfx.update_dynamic_vertex_buffer(
            Handle,
            (uint)startVertex,
            BgfxUtils.MakeRef(data)
        );

        VertexCount = data.Length;
    }

    protected override void Free()
    {
        if (Handle.Valid)
        {
            Bgfx.destroy_dynamic_vertex_buffer(Handle);    
        }
    }

    internal Bgfx.DynamicVertexBufferHandle Handle { get; }
}
