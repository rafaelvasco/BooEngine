using Boo.Common.Math;
using Boo.Engine.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class BooTileset : BooAsset
{
    public int TileSize { get; }

    public ref Quad this[int index]
    {
        get
        {
            index = Calc.Clamp(index, 0, _quads.Length - 1);

            return ref _quads[index];
        }
    }

    public Texture2D Texture { get; }

    internal BooTileset(string id, Texture2D texture, int tileSize) : base(id)
    {
        Texture = texture;
        TileSize = tileSize;

        int tileCountX = texture.Width / tileSize;
        int tileCountY = texture.Height / tileSize;

        _quads = new Quad[tileCountX * tileCountY];

        int index = 0;
        
        for (int j = 0; j < tileCountY; j++)
        {
            for (int i = 0; i < tileCountX; i++)
            {
                var quad = new Quad(texture, new Rect(i * tileSize, j * tileSize, tileSize, tileSize));
                _quads[index++] = quad;
            }
        }
    }

    private readonly Quad[] _quads;
    protected override void Free()
    {
        Texture.Dispose();
    }
}