using System.Runtime.InteropServices;

namespace Boo.Engine.Graphics;

public unsafe class StaticMesh
{
    private VertexBuffer? _vertexBuffer;
    private IndexBuffer? _indexBuffer;
    private int _vertexIndex;
    private int _indiceIndex;
    private int _indexCount;
    private Vertex[] _vertices;
    private short[] _indices;
    private bool _open;
    
    public int VertexCount => _vertexIndex;
    public int IndexCount => _indexCount;
    

    public StaticMesh()
    {
        _vertices = new Vertex[512 * 4];
        _indices = new short[512 * 6];
    }

    public void Open()
    {
        if (_open)
        {
            throw new InvalidOperationException();
        }
        
        _open = true;
        
        _vertexIndex = 0;
        _indiceIndex = 0;
        _indexCount = 0;

        if (_vertexBuffer == null) return;
        _vertexBuffer.Dispose();
        _vertexBuffer = null;

        if (_indexBuffer == null) return;
        _indexBuffer.Dispose();
        _indexBuffer = null;
        
    }

    public void PushTriangle(Vertex vertex1, Vertex vertex2, Vertex vertex3)
    {
        if (!_open)
        {
            throw new InvalidOperationException();
        }
        
        IncreaseBufferIfNeeded(3);

        fixed (Vertex* p = &MemoryMarshal.GetArrayDataReference(_vertices))
        fixed(short* i = &MemoryMarshal.GetArrayDataReference(_indices))    
        {
            int vi = _vertexIndex;
            int ii = _indiceIndex;    
            
            *(p + vi) = vertex1;
            *(p + vi + 1) = vertex2;
            *(p + vi + 2) = vertex3;

            short baseIndex = (short)ii;
            
            *(i + ii) = baseIndex;
            *(i + ii + 1) = (short)(baseIndex + 1);
            *(i + ii + 2) = (short)(baseIndex + 2);
        }

        unchecked
        {
            _vertexIndex += 3;
            _indiceIndex += 3;
            _indexCount += 3;
        }
    }

    public void PushQuad(ref Quad quad)
    {
        if (!_open)
        {
            throw new InvalidOperationException();
        }
        
        IncreaseBufferIfNeeded(4);

        fixed (Vertex* p = &MemoryMarshal.GetArrayDataReference(_vertices))
        fixed(short* i = &MemoryMarshal.GetArrayDataReference(_indices))    
        {
            int vi = _vertexIndex;
            int ii = _indiceIndex;    
            
            *(p + vi) = quad.TopLeft;
            *(p + vi + 1) = quad.TopRight;
            *(p + vi + 2) = quad.BottomRight;
            *(p + vi + 3) = quad.BottomLeft; 
            
            short baseIndex = (short)ii;
            
            *(i + _indexCount) = baseIndex;
            *(i + _indexCount + 1) = (short)(baseIndex + 1);
            *(i + _indexCount + 2) = (short)(baseIndex + 2);
            *(i + _indexCount + 3) = baseIndex;
            *(i + _indexCount + 4) = (short)(baseIndex + 2);
            *(i + _indexCount + 5) = (short)(baseIndex + 3);
        }

        unchecked
        {
            _vertexIndex += 4;
            _indiceIndex += 4;
            _indexCount += 6;
        }
    }

    public void Close()
    {
        if (!_open)
        {
            throw new InvalidOperationException();
        }
        
        _vertexBuffer = new VertexBuffer(Vertex.VertexLayout, _vertices);
        _indexBuffer = new IndexBuffer(_indices);        
        _open = false;
    }

    public void Submit()
    {
        Submit(0, VertexCount, 0, IndexCount);
    }

    public void Submit(int startingVertexIndex, int vertexCount,
        int startingIndiceIndex, int indexCount)
    {
        if (_open || _vertexBuffer == null || _indexBuffer == null)
        {
            throw new InvalidOperationException();
        }
        
        BooGraphics.SetIndexBuffer(_indexBuffer, startingIndiceIndex, indexCount);
        BooGraphics.SetVertexBuffer(_vertexBuffer, startingVertexIndex, vertexCount);
    }

    private void IncreaseBufferIfNeeded(int delta)
    {
        if (_vertexIndex + delta <= _vertices.Length) return;
        Array.Resize(ref _vertices, _vertices.Length * 2);
        Array.Resize(ref _indices, _indices.Length * 2);
    }
}