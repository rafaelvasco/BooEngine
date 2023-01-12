using Boo.Common;
using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public enum ValueUpdateMode
{
    Discrete,
    Interpolated
}

public enum AnimationLoopMode
{
    OneShot,
    Loop,
    PingPong
}

public class Animation
{
    public List<BaseAnimationTrack> Tracks { get; }

    private BooNode TargetNode { get; }

    internal Animation(BooNode targetNode)
    {
        TargetNode = targetNode;
        Tracks = new List<BaseAnimationTrack>();
    }

    public T AddTrack<T>() where T : BaseAnimationTrack
    {
        var track = (T)Activator.CreateInstance(typeof(T), args: TargetNode)!;
        Tracks.Add(track);
        track.Active = _active;
        return track;
    }

    public void Reset()
    {
        foreach (var animationTrack in Tracks)
        {
            animationTrack.Reset();
        }
    }

    public void SetActive(bool active)
    {
        _active = active;

        foreach (var animationTrack in Tracks)
        {
            animationTrack.Active = active;
        }
    }

    internal void Update(GameTime time)
    {
        if (!_active)
        {
            return;
        }

        foreach (var animationTrack in Tracks)
        {
            animationTrack.Animate(time);
        }
    }

    private bool _active = true;
}

public class AnimationFrame<T>
{
    public float Duration;
    public T? Value;

    public AnimationFrame()
    {
        Value = default;
    }
}

public abstract class BaseAnimationTrack
{
    protected BooNode _parent;

    protected BaseAnimationTrack(BooNode parent)
    {
        _parent = parent;
    }
    
    public AnimationLoopMode AnimationMode
    {
        get => _loopMode;
        set
        {
            if (_loopMode != value)
            {
                _loopMode = value;
                Reset();
            }
        }
    }

    public int KeyFrameIndex => _keyFrameIndex;

    public Ease EaseMode { get; set; } = Ease.Linear;

    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Discrete;

    public bool InterpolateLastFrameToFirst { get; set; } = true;

    public bool Active { get; set; }

    public abstract void Animate(GameTime time);

    protected abstract void InterpolateValue();

    public abstract void Reset();

    protected float _time;
    protected int _endKeyFrameIndex;
    protected int _keyFrameIndex;
    protected int _animationDirection = 1;
    protected AnimationLoopMode _loopMode = AnimationLoopMode.Loop;
}

public abstract class AnimationTrack<T> : BaseAnimationTrack
{
    private const int DEFAULT_MAX_FRAMES = 50;

    public ref T? Value
    {
        get
        {
            switch (UpdateMode)
            {
                case ValueUpdateMode.Discrete:
                    return ref _frames[_keyFrameIndex].Value;
                case ValueUpdateMode.Interpolated:
                    return ref _interpolatedValue;
            }

            return ref _frames[_keyFrameIndex].Value;
        }
    }


    public AnimationTrack(BooNode parent) : base(parent)
    {
        _frames = new AnimationFrame<T>[DEFAULT_MAX_FRAMES];

        for (int i = 0; i < DEFAULT_MAX_FRAMES; i++)
        {
            _frames[i] = new AnimationFrame<T>();
        }
    }

    public virtual AnimationTrack<T> SetKeyFrame(int index, float durationSec, T value)
    {
        if (index > _frames.Length - 1)
        {
            Array.Resize(ref _frames, _frames.Length * 2);
        }

        index = Calc.Max(index, 0);

        _frames[index].Value = value;
        _frames[index].Duration = durationSec;

        _endKeyFrameIndex = Calc.Max(index, _endKeyFrameIndex);

        return this;
    }

    public override void Reset()
    {
        _keyFrameIndex = 0;
        _time = 0f;
        _animationDirection = 1;
    }

    public override void Animate(GameTime time)
    {
        if (!Active)
        {
            return;
        }

        if (_endKeyFrameIndex == 0)
        {
            return;
        }

        float dt = (float)time.ElapsedGameTime.TotalSeconds;

        _time += dt;

        if (_time >= _frames[_keyFrameIndex].Duration)
        {
            _time = 0f;
            _keyFrameIndex += _animationDirection;
        }

        if (_keyFrameIndex < 0 || _keyFrameIndex > _endKeyFrameIndex)
        {
            switch (AnimationMode)
            {
                case AnimationLoopMode.OneShot:
                    _keyFrameIndex = _endKeyFrameIndex;
                    Active = false;
                    _time = 0.0f;
                    break;
                case AnimationLoopMode.Loop:
                    Reset();
                    break;
                case AnimationLoopMode.PingPong:
                    _animationDirection = -_animationDirection;
                    _keyFrameIndex = Calc.Clamp(_keyFrameIndex, 0, _endKeyFrameIndex);
                    _time = 0.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (UpdateMode == ValueUpdateMode.Interpolated)
        {
            InterpolateValue();
        }
    }

    protected AnimationFrame<T>[] _frames;
    protected T? _interpolatedValue;
}

public class TimelineAnimation : BooComponent
{
    public TimelineAnimation()
    {
        Animations = new Dictionary<string, Animation>();
    }

    public Dictionary<string, Animation> Animations { get; }

    public Animation? CurrentAnimation { get; private set; }

    public override void OnAttached(BooNode parent)
    {
        AddAnimation("Default");
    }

    public Animation AddAnimation(string name)
    {
        if (Parent == null)
        {
            throw new BooException("AnimationComponent is Unatached. Can't add animations");
        }

        var animation = new Animation(Parent);

        Animations.Add(name, animation);

        return animation;
    }

    public void SetAnimation(string name)
    {
        if (Animations.TryGetValue(name, out Animation? anim))
        {
            CurrentAnimation = anim;
            CurrentAnimation.Reset();
            CurrentAnimation.SetActive(true);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(name));
        }
    }

    public override void Update(GameTime time)
    {
        if (Parent != null)
        {
            CurrentAnimation?.Update(time);
        }
    }
}