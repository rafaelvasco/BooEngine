using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Content;

namespace Boo.Engine.Toolkit;


public class BooTileMap : BooNode
{
    public BooTileset Tileset { get; set; }

    public BooTileMap(BooTileset tileset, int[] tiles, int mapWidth, int mapHeight) : base("Tilemap")
    {
        Tileset = tileset;
        _tiles = tiles;

        _width = mapWidth;
        _height = mapHeight;
    }

    internal static BooTileMap LoadFromDefinition(BooTileMapDefinition definition)
    {
        if (definition.TileSet == null)
        {
            throw new BooException("Invalid BooTileMap definiion: Missing Tileset property");
        }
        
        if (definition.Tiles == null || definition.Tiles.Length == 0)
        {
            throw new BooException("Invalid BooTileMap definition: Missing or empty Tiles attribute");
        }

        var tileset = BooContent.Get<BooTileset>(definition.TileSet);
        

        int[] tiles = new int[definition.Tiles.Length];

        int index = 0;
        
        foreach (var tileDefinition in definition.Tiles)
        {
            tiles[index++] = Calc.Pack3(tileDefinition.TileIndex, tileDefinition.TileX, tileDefinition.TileY);
        }

        var tilemap = new BooTileMap(tileset, tiles, definition.MapWidth, definition.MapHeight);

        return tilemap;
    }

    public override RectF BoundingRect => new(X, Y, _width * Tileset.TileSize, _height * Tileset.TileSize);

    public override void Process(GameTime time)
    {
    }

    public override void Draw(BooCanvas canvas)
    {
        var texture = Tileset.Texture;
        var tileSize = Tileset.TileSize * 4;

        float x = (Parent?.X ?? 0) + X;
        float y = (Parent?.Y ?? 0) + Y;

        for (int i = 0; i < _tiles.Length; ++i)
        {
            var tile = _tiles[i];

            (int tileIndex, int tileX, int tileY) = Calc.Unpack(tile);

            var quad = Tileset[tileIndex];

            quad.SetXYWH(x + tileX * tileSize, y + tileY * tileSize, tileSize, tileSize, 0f, 0f);

            canvas.Draw(texture, ref quad);
        }
    }

    private int[] _tiles;
    private int _width;
    private int _height;
}