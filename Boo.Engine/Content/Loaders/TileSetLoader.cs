using Boo.Common.Content;
using Boo.Engine.Graphics;
using Boo.Engine.Toolkit;

namespace Boo.Engine.Content;

internal class TileSetLoader : AssetLoader<BooTileset, TileSetData, DefaultFileManifestInfo>
{
    public override BooTileset LoadFromData(TileSetData data)
    {
        var loader = BooContent.GetLoader<Texture2D>();

        var texture = ((loader as TextureLoader)!).LoadFromData(data.ImageData);

        var tileSet = new BooTileset(data.Id, texture, data.TileSize);

        return tileSet;
    }
}