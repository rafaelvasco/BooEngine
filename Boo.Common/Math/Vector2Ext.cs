using System.Numerics;

namespace Boo.Common.Math;

public static class Vector2Ext
{
    public static Vector2 Normalize(this Vector2 v)
    {
        var val = 1.0f / Calc.Sqrt(v.X * v.X + v.Y * v.Y);

        return new Vector2(v.X * val, v.Y * val);
    }

    public static void Deconstruct(this Vector2 v, out float x, out float y)
    {
        x = v.X;
        y = v.Y;
    }
}
