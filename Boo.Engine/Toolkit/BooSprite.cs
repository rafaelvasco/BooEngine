using System.Numerics;
using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class BooSprite : BooNode
{
    private Quad _quad;

    private Size _size;

    public Vector2 Origin { get; set; } = new(0.5f, 0.5f);

    public Size Size
    {
        get => new((int)_quad.Width, (int)_quad.Height);
        set => _size = value;
    }

    public Texture2D? Texture { get; }

    public Color ColorTint { get; set; } = Color.White;

    public float Opacity { get; set; } = 1.0f;

    public BooSprite(Texture2D texture) : base("Texture2D")
    {
        Texture = texture;
        _quad = new Quad(Texture);

        _size = new Size((int)_quad.Width, (int)_quad.Height);
    }

    public static BooSprite LoadFromDefinition(BooSpriteDefinition definition)
    {
        if (definition.Texture == null)
        {
            throw new BooException("Invalid SpriteDefinition: Missing Texture attribute.");
        }
        
        var sprite = new BooSprite(BooContent.Get<Texture2D>(definition.Texture));

        sprite.ColorTint = definition.ColorTint;
        sprite.Origin = definition.Origin;
        
        if (!definition.Size.IsEmpty && definition.Size.Width > 0 && definition.Size.Height > 0)
        {
            sprite.Size = definition.Size;
        }

        if (!definition.SourceRect.IsEmpty)
        {
            sprite.SetSourceRect(definition.SourceRect);
        }

        return sprite;
    }
    
    public void SetSourceRect(Rect rect)
    {
        if (Texture == null)
        {
            throw new InvalidOperationException();
        }

        _quad.SetRegion(Texture, rect);
    }


    public override RectF BoundingRect =>
        new(X - Size.Width * Origin.X, Y - Size.Height * Origin.Y, Size.Width, Size.Height);

    public override void Process(GameTime time)
    {
    }

    public override void Draw(BooCanvas canvas)
    {
        if (!Visible || Texture == null)
        {
            return;
        }

        float x = (Parent?.X ?? 0) + X;
        float y = (Parent?.Y ?? 0) + Y;

        _quad.SetXYWH(x, y, _size.Width, _size.Height, Origin.X, Origin.Y);

        if (Opacity < 1.0f)
        {
            ColorTint = ColorTint.WithAlpha(Opacity);
        }

        _quad.SetColor(ColorTint);

        canvas.Draw(Texture, ref _quad);
    }
}