using System.Numerics;

namespace Boo.Engine.Toolkit;

public class PositionAnimationTrack : AnimationTrack<Vector2>
{
    public PositionAnimationTrack(BooNode parent) : base(parent)
    {
    }
    
    public override void Animate(GameTime time)
    {
        base.Animate(time);

        _parent.X = Value.X;
        _parent.Y = Value.Y;
    }

    protected override void InterpolateValue()
    {
        var currentFrame = _frames[_keyFrameIndex];

        ref var currentPos = ref currentFrame.Value;

        var currentFrameDuration = currentFrame.Duration;

        ref Vector2 nextPos = ref Value;

        if ((_animationDirection == 1 && _keyFrameIndex < _endKeyFrameIndex) ||
            (_animationDirection == -1 && _keyFrameIndex > 0))
        {
            nextPos = ref _frames[_keyFrameIndex + _animationDirection].Value;
            _interpolatedValue = new Vector2(
                Interpolator.Calculate(EaseMode, currentPos.X, nextPos.X, _time, currentFrameDuration),
                Interpolator.Calculate(EaseMode, currentPos.Y, nextPos.Y, _time, currentFrameDuration)
            );
        }
        else
        {
            if (AnimationMode == AnimationLoopMode.Loop && InterpolateLastFrameToFirst)
            {
                if (_animationDirection == 1 && _keyFrameIndex == _endKeyFrameIndex)
                {
                    nextPos = ref _frames[0].Value;
                }
                else if (_animationDirection == -1 && _keyFrameIndex == 0)
                {
                    nextPos = ref _frames[_endKeyFrameIndex].Value;
                }

                _interpolatedValue = new Vector2(
                    Interpolator.Calculate(EaseMode, currentPos.X, nextPos.X, _time, currentFrameDuration),
                    Interpolator.Calculate(EaseMode, currentPos.Y, nextPos.Y, _time, currentFrameDuration)
                );
            }
            else
            {
                _interpolatedValue = _frames[_keyFrameIndex].Value;
            }
        }
    }

    
}