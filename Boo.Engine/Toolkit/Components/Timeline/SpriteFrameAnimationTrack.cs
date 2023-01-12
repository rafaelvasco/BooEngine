using Boo.Common;
using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public class SpriteFrameAnimationTrack : AnimationTrack<Rect>
{
    private BooSprite _sprite;

    public SpriteFrameAnimationTrack(BooNode parent) : base(parent)
    {
        if (parent is not BooSprite sprite)
        {
            throw new BooException("SpriteFrameAnimation can only be added to a BooNode of type BooSprite.");
        }

        _sprite = sprite;
    }

    public override void Animate(GameTime time)
    {
        base.Animate(time);

        _sprite.SetSourceRect(Value);
    }

    protected override void InterpolateValue()
    {
        var currentFrame = _frames[_keyFrameIndex];

        var x = currentFrame.Value.X;
        var y = currentFrame.Value.Y;

        var currentFrameDuration = _frames[_keyFrameIndex].Duration;

        ref Rect nextRect = ref Value;

        if ((_animationDirection == 1 && _keyFrameIndex < _endKeyFrameIndex) ||
            (_animationDirection == -1 && _keyFrameIndex > 0))
        {
            nextRect = _frames[_keyFrameIndex + _animationDirection].Value;
            _interpolatedValue =
                new Rect(
                    (int)Interpolator.Calculate(EaseMode, x, nextRect.X, _time, currentFrameDuration),
                    (int)Interpolator.Calculate(EaseMode, y, nextRect.Y, _time, currentFrameDuration),
                    Value.Width,
                    Value.Height);
        }
        else
        {
            if (AnimationMode == AnimationLoopMode.Loop && InterpolateLastFrameToFirst)
            {
                if (_animationDirection == 1 && _keyFrameIndex == _endKeyFrameIndex)
                {
                    nextRect = _frames[0].Value;
                }
                else if (_animationDirection == -1 && _keyFrameIndex == 0)
                {
                    nextRect = _frames[_endKeyFrameIndex].Value;
                }

                _interpolatedValue =
                    new Rect(
                        (int)Interpolator.Calculate(EaseMode, x, nextRect.X, _time, currentFrameDuration),
                        (int)Interpolator.Calculate(EaseMode, y, nextRect.Y, _time, currentFrameDuration),
                        Value.Width,
                        Value.Height);
            }
            else
            {
                _interpolatedValue = _frames[_keyFrameIndex].Value;
            }
        }
    }
}