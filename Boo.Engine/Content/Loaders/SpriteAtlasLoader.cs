using Boo.Common.Content;
using Boo.Engine.Graphics;
using Boo.Engine.Toolkit;

namespace Boo.Engine.Content;

internal class SpriteAtlasLoader : AssetLoader<BooSpriteAtlas, SpriteAtlasData, DefaultFileManifestInfo>
{
    public override BooSpriteAtlas LoadFromData(SpriteAtlasData data)
    {
        var loader = BooContent.GetLoader<Texture2D>();

        var texture = ((loader as TextureLoader)!).LoadFromData(data.ImageData);

        var spriteAtlas = new BooSpriteAtlas(data.Id, texture);

        foreach (var (spriteName, spriteRegion) in data.Regions)
        {
            spriteAtlas.AddSprite(spriteName, spriteRegion);
        }

        return spriteAtlas;
    }
}