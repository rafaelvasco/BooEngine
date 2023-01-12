using System.Numerics;
using Boo.Common.Math;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class BooViewport : BooContainer
{
    public RectF Viewport
    {
        get => _canvasView.Viewport;
        set => _canvasView.Viewport = new RectF(BooGraphics.BackbufferWidth * value.X,
            BooGraphics.BackbufferHeight * value.Y, BooGraphics.BackbufferWidth * value.Width,
            BooGraphics.BackbufferHeight * value.Height);
    }

    public Size Size
    {
        get => _size;
        set
        {
            _size = value;
            _canvasView.Projection = Matrix4x4.CreateOrthographicOffCenter(0, _size.Width, _size.Height,
                0,
                -1.0f, 1.0f);
        }
    }


    private readonly BooCanvasView _canvasView;
    private Size _size;

    public BooViewport(string name) : base(name)
    {
        _canvasView = new BooCanvasView();
        

        Viewport = new RectF(0f, 0f, 1f, 1f);
        Size = new Size(BooGraphics.BackbufferWidth, BooGraphics.BackbufferHeight);
    }

    public override RectF BoundingRect => new(
        0,
        0,
        Size.Width,
        Size.Height
    );

    public override void Process(GameTime time)
    {
    }

    public override void Draw(BooCanvas canvas)
    {
        canvas.BeginView(_canvasView);
        
        foreach (var node in Children)
        {
            node.Draw(canvas);
        }
        
        canvas.EndView();
    }
}