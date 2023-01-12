namespace Boo.Common.Math;

public struct Size
{
    public int Width { get; set; }
    public int Height { get; set; }

    public bool IsEmpty => Width == 0 && Height == 0;

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }
}