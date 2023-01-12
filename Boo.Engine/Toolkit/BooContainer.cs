using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public class BooContainer : BooNode
{
    public List<BooNode> Children { get; }

    public BooContainer(string name) : base(name)
    {
        Children = new List<BooNode>();
    }

    public BooNode AddChild(BooNode child)
    {
        child.Parent = this;
        Children.Add(child);

        if (child.BoundingRect.Left < _minX)
        {
            _minX = child.BoundingRect.Left;
        }

        if (child.BoundingRect.Right > _maxX)
        {
            _maxX = child.BoundingRect.Right;
        }

        if (child.BoundingRect.Top < _minY)
        {
            _minY = child.BoundingRect.Top;
        }

        if (child.BoundingRect.Bottom > _maxY)
        {
            _maxY = child.BoundingRect.Bottom;
        }

        return child;
    }

    internal override void InternalProcess(GameTime time)
    {
        base.InternalProcess(time);

        foreach (var booNode in Children)
        {
            booNode.InternalProcess(time);
        }
    }

    public override void Process(GameTime time)
    {
    }

    public override RectF BoundingRect => new (_minX, _minY, _maxX - _minX, _maxY - _minY);

    public override void Draw(BooCanvas canvas)
    {
        if (!Visible)
        {
            return;
        }

        foreach (var booNode in Children)
        {
            booNode.Draw(canvas);
        }
    }

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;
}