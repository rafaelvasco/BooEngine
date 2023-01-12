using Boo.Common.Math;
using Boo.Engine.Input;

namespace Boo.Engine.Toolkit;

public class TileMovement : BooComponent
{
    /// <summary>
    /// How much to move on each horizontal step;
    /// </summary>
    public int GridWidth { get; set; } = 64;

    /// <summary>
    /// How much to move on each vertical step;
    /// </summary>
    public int GridHeight { get; set; } = 64;

    /// <summary>
    /// Move duration in seconds;
    /// </summary>
    public float MoveDuration { get; set; } = 0.1f;


    /// <summary>
    /// What interpolation mode to use while moving;
    /// </summary>
    public Ease EaseMode { get; set; } = Ease.Linear;

    public override void OnAttached(BooNode parent)
    {
        base.OnAttached(parent);

        _startX = Parent!.X;
        _startX = Parent!.Y;
    }

    public override void Update(GameTime time)
    {
        if (Parent == null)
        {
            return;
        }

        float dt = time.FrameDeltaSec;

        if (!_moving)
        {
            if (BooInput.CheckControl("InputLeft"))
            {
                _startX = Parent.X;
                _startY = Parent.Y;
                _nextTargetX = Parent.X - GridWidth;
                _nextTargetY = Parent.Y;
                _moving = true;
            }
            else if (BooInput.CheckControl("InputRight"))
            {
                _startX = Parent.X;
                _startY = Parent.Y;
                _nextTargetX = Parent.X + GridWidth;
                _nextTargetY = Parent.Y;
                _moving = true;
            }

            if (BooInput.CheckControl("InputUp"))
            {
                _startX = Parent.X;
                _startY = Parent.Y;
                _nextTargetX = Parent.X;
                _nextTargetY = Parent.Y - GridHeight;
                _moving = true;
            }
            else if (BooInput.CheckControl("InputDown"))
            {
                _startX = Parent.X;
                _startY = Parent.Y;
                _nextTargetX = Parent.X;
                _nextTargetY = Parent.Y + GridHeight;
                _moving = true;
            }
        }
        else
        {
            _time += dt;

            if (!Calc.ApproximatelyEqual(_nextTargetX, Parent.X))
            {
                Parent.X = Interpolator.Calculate(EaseMode, _startX, _nextTargetX, _time, MoveDuration);

                if (Calc.Abs(Parent.X - _nextTargetX) < 1.0f)
                {
                    _time = 0f;
                    Parent.X = _nextTargetX;
                    _moving = false;
                }
            }

            if (!Calc.ApproximatelyEqual(_nextTargetY, Parent.Y))
            {
                Parent.Y = Interpolator.Calculate(Ease.Linear, _startY, _nextTargetY, _time, MoveDuration);

                if (Calc.Abs(Parent.Y - _nextTargetY) < 1.0f)
                {
                    _time = 0f;
                    Parent.Y = _nextTargetY;
                    _moving = false;
                }
            }
        }
    }

    private float _time;
    private bool _moving;
    private float _nextTargetX;
    private float _nextTargetY;
    private float _startX;
    private float _startY;
}