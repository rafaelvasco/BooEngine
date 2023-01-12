using Boo.Common.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Content;

internal class TextureLoader : AssetLoader<Texture2D, ImageData, DefaultFileManifestInfo>
{
    public override Texture2D LoadFromData(ImageData data)
    {
        BooBlitter.Begin(data.Data, data.Width, data.Height);
        BooBlitter.ConvertRgbaToBgra();
        BooBlitter.End();
        
        var texture = BooGraphics.CreateTexture(data.Id, new Texture2DCreateInfo()
        {
            Data = data.Data,
            Width = data.Width,
            Height = data.Height,
            BytesPerPixel = data.BytesPerPixel
        });

        return texture;
    }
}