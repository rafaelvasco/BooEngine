using Boo.Common;

namespace Boo.Engine.Toolkit;

public class CallMethodAnimationTrack : AnimationTrack<string>
{
    private readonly Dictionary<string, Action> _callbackMap;

    public CallMethodAnimationTrack(BooNode parent) : base(parent)
    {
        _callbackMap = new Dictionary<string, Action>();
    }

    public override void Animate(GameTime time)
    {
        base.Animate(time);

        if (Value != null && _callbackMap.TryGetValue(Value, out Action? action))
        {
            action.Invoke();
        }
    }

    public override AnimationTrack<string> SetKeyFrame(int index, float durationSec, string value)
    {
        Type type = _parent.GetType();

        var methodInfo = type.GetMethod(value);
        if (methodInfo == null) throw new BooException($"Cannot set keyframe: Inexistent method name: {value}");
        
        var action = Delegate.CreateDelegate(typeof(Action), _parent, methodInfo);
            
        _callbackMap.Add(value, (Action)action);
        
        base.SetKeyFrame(index, durationSec, value);

        return this;
    }

    protected override void InterpolateValue()
    {
        _interpolatedValue = _frames[_keyFrameIndex].Value;
    }
}