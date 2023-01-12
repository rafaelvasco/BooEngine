using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Boo.Engine.Graphics;

public struct Vertex
{
    public uint Color;
    public float X;
    public float Y;
    public float Z;
    public float U;
    public float V;

    public static readonly VertexLayout VertexLayout;
    
    private static readonly int _stride;

    public static int Stride => _stride;

    public Vertex( uint color, float x, float y, float z, float u, float v)
    {
        Color = color;
        X = x;
        Y = y;
        Z = z;
        U = u;
        V = v;
    }

    static Vertex()
    {
        VertexLayout = new VertexLayout(
            new VertexAttribute(VertexAttributeRole.Color0, 4, VertexAttributeType.UInt8, true, false),
            new VertexAttribute(VertexAttributeRole.Position, 3, VertexAttributeType.Float, false, false),
            new VertexAttribute(VertexAttributeRole.TexCoord0, 2, VertexAttributeType.Float, false, false)
        );
        
        _stride = Unsafe.SizeOf<Vertex>();
    }
}
