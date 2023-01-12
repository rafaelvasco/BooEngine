using System.Runtime.CompilerServices;
using Boo.Common.Math;

namespace Boo.Engine.Graphics;

public struct Quad
{
    public Vertex TopLeft;
    public Vertex TopRight;
    public Vertex BottomRight;
    public Vertex BottomLeft;

    public static readonly int SizeInBytes = Unsafe.SizeOf<Quad>();

    public float Width => Calc.Abs(TopRight.X - TopLeft.X);
    public float Height => Calc.Abs(BottomRight.Y - TopRight.Y);

    public Quad(
        float x1, float y1, float z1, Color color1, float t1x, float t1y,
        float x2, float y2, float z2, Color color2, float t2x, float t2y,
        float x3, float y3, float z3, Color color3, float t3x, float t3y,
        float x4, float y4, float z4, Color color4, float t4x, float t4y
    )
    {
        TopLeft = new Vertex(color1, x1, y1, z1, t1x, t1y);
        TopRight = new Vertex(color2, x2, y2, z2, t2x, t2y);
        BottomRight = new Vertex(color3, x3, y3, z3, t3x, t3y);
        BottomLeft = new Vertex(color4, x4, y4, z4, t4x, t4y);
    }

    public Quad(Texture2D texture, Rect srcRect = default)
    {
        TopLeft = default;
        TopRight = default;
        BottomRight = default;
        BottomLeft = default;

        SetRegion(texture, srcRect);

        float w = texture.Width;
        float h = texture.Height;

        if (!srcRect.IsEmpty)
        {
            w = srcRect.Width;
            h = srcRect.Height;
        }

        TopLeft.X = 0;
        TopLeft.Y = 0;
        TopLeft.Color = 0xFFFFFFFF;

        TopRight.X = w;
        TopRight.Y = 0;
        TopRight.Color = 0xFFFFFFFF;

        BottomRight.X = w;
        BottomRight.Y = h;
        BottomRight.Color = 0xFFFFFFFF;

        BottomLeft.X = 0;
        BottomLeft.Y = h;
        BottomLeft.Color = 0xFFFFFFFF;
    }

    public void SetColor(Color color)
    {
        TopLeft.Color = color;
        TopRight.Color = color;
        BottomRight.Color = color;
        BottomLeft.Color = color;
    }

    public void SetColors(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
    {
        TopLeft.Color = topLeft;
        TopRight.Color = topRight;
        BottomRight.Color = bottomRight;
        BottomLeft.Color = bottomLeft;
    }

    public void SetZ(float depth)
    {
        TopLeft.Z = depth;
        TopRight.Z = depth;
        BottomRight.Z = depth;
        BottomLeft.Z = depth;
    }

    public void SetXY(float x, float y, float originX = 0f, float originY= 0f)
    {
        SetXYWH(x, y, Width, Height, originX, originY);
    }

    public void SetXYWH(float x, float y, float w, float h, float originX, float originY)
    {
        var x0 = x - w * originX;
        var y0 = y - h * originY;
        var x1 = x0 + w;
        var y1 = y0 + h;

        TopLeft.X = x0;
        TopLeft.Y = y0;

        TopRight.X = x1;
        TopRight.Y = y0;

        BottomRight.X = x1;
        BottomRight.Y = y1;

        BottomLeft.X = x0;
        BottomLeft.Y = y1;
    }

    public void SetXYWHR(float x, float y, float w, float h, float originX, float originY, float sin, float cos)
    {
        var dx = w * originX;
        var dy = h * originY;

        TopLeft.X = x + dx * cos - dy * sin;
        TopLeft.Y = y + dx * sin + dy * cos;

        TopRight.X = x + (dx + w) * cos - dy * sin;
        TopRight.Y = y + (dx + w) * sin + dy * cos;

        BottomLeft.X = x + dx * cos - (dy + h) * sin;
        BottomLeft.Y = y + dx * sin + (dy + h) * cos;

        BottomRight.X = x + (dx + w) * cos - (dy + h) * sin;
        BottomRight.Y = y + (dx + w) * sin + (dy + h) * cos;
    }

    public void SetRegion(Texture2D texture, Rect region)
    {
        float ax, ay, bx, by;

        if (region.IsEmpty)
        {
            ax = 0;
            ay = 0;
            bx = 1;
            by = 1;
        }
        else
        {
            float invTexW = 1.0f / texture.Width;
            float invTexH = 1.0f / texture.Height;

            ax = region.Left * invTexW;
            ay = region.Top * invTexH;
            bx = region.Right * invTexW;
            by = region.Bottom * invTexH;
        }

        TopLeft.U = ax;
        TopLeft.V = ay;
        TopRight.U = bx;
        TopRight.V = ay;
        BottomRight.U = bx;
        BottomRight.V = by;
        BottomLeft.U = ax;
        BottomLeft.V = by;
    }

    public override string ToString()
    {
        return $"{TopLeft};{TopRight};{BottomRight};{BottomLeft}";
    }
}