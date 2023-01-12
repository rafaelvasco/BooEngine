using System.Runtime.CompilerServices;
using Boo.Native;

namespace Boo.Engine.Graphics;

public unsafe struct TransientVertexBuffer
{
    internal readonly Bgfx.TransientVertexBuffer Handle;

    public TransientVertexBuffer(Span<Vertex> vertices, VertexLayout layout, int vertexByteSize)
    {
        var handle = new Bgfx.TransientVertexBuffer();
        
        Bgfx.alloc_transient_vertex_buffer(&handle, (uint)vertices.Length, Vertex.VertexLayout.LayoutData);
        
        var dataSize = vertices.Length * vertexByteSize;

        Unsafe.CopyBlockUnaligned(handle.data, Unsafe.AsPointer(ref vertices[0]), (uint)dataSize);

        Handle = handle;
    }
}