using System.Runtime.InteropServices;
using Boo.Common.Math;

namespace Boo.Engine.Graphics;

public class DynamicQuadMesh
{
    public int VertexCount => _vertexIndex;
    
    public int MaxVertices { get; }
    
    public DynamicQuadMesh(int maxQuads = 2048)
    {
        if (!Calc.IsPowerOfTwo(maxQuads))
        {
            maxQuads = Calc.NextPowerOfTwo(maxQuads);
        }

        MaxVertices = maxQuads * 4;

        _vertices = new Vertex[MaxVertices];

        BuildIndices(maxQuads);
    }

    public void Reset()
    {
        _indiceIndex = 0;
        _vertexIndex = 0;
    }

    public unsafe void PushQuad(ref Quad quad)
    {
        fixed (Vertex* p = &MemoryMarshal.GetArrayDataReference(_vertices))
        {
            int index = _vertexIndex;

            *(p + index) = quad.TopLeft;
            *(p + index + 1) = quad.TopRight;
            *(p + index + 2) = quad.BottomRight;
            *(p + index + 3) = quad.BottomLeft;
        }

        unchecked
        {
            _vertexIndex += 4;
            _indiceIndex += 6;
        }
    }

    public void Submit()
    {
        Submit(0, _vertexIndex, 0, _indiceIndex);
    }

    public void Submit(int startingVertexIndex, int vertexCount, int startingIndiceIndex, int indexCount)
    {
        if (_dynamicIndexBuffer == null)
        {
            throw new InvalidOperationException();
        }

        BooGraphics.SetDynamicIndexBuffer(_dynamicIndexBuffer, startingIndiceIndex,
            indexCount > 0 ? indexCount : _indiceIndex);

        var verticesSpan = new Span<Vertex>(_vertices, startingVertexIndex,
            vertexCount > 0 ? vertexCount : _vertexIndex);

        var transientVbo = new TransientVertexBuffer(verticesSpan, Vertex.VertexLayout, Vertex.Stride);

        BooGraphics.SetTransientVertexBuffer(ref transientVbo, vertexCount);
    }
    

    private void BuildIndices(int maxQuads)
    {
        int countIndices = maxQuads * 6;

        var indices = new short[countIndices];

        var index = 0;

        for (int i = 0; i < maxQuads; i++, index += 6)
        {
            indices[index + 0] = (short)(i * 4 + 0);
            indices[index + 1] = (short)(i * 4 + 1);
            indices[index + 2] = (short)(i * 4 + 2);

            indices[index + 3] = (short)(i * 4 + 0);
            indices[index + 4] = (short)(i * 4 + 2);
            indices[index + 5] = (short)(i * 4 + 3);
        }

        if (_dynamicIndexBuffer == null)
        {
            _dynamicIndexBuffer = new DynamicIndexBuffer(indices);
        }
        else
        {
            _dynamicIndexBuffer.SetData(indices);
        }
    }

    private DynamicIndexBuffer? _dynamicIndexBuffer;

    private int _vertexIndex;
    private int _indiceIndex;

    private Vertex[] _vertices;
}