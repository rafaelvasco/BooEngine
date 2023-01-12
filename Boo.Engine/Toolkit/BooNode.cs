
using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public abstract class BooNode
{
    private static int SUID;
    
    protected BooNode(string name)
    {
        UId = SUID++;
        Name = $"{name}_${UId}";
    }

    public int UId { get; internal set; }

    public string Name { get; set; }

    public bool Visible { get; set; } = true;

    public float X { get; set; }

    public float Y { get; set; }

    public abstract RectF BoundingRect { get; }
    
    public BooContainer? Parent { get; internal set; }

    public T AddComponent<T>() where T : BooComponent
    {
        if (_componentsMap == null || _components == null)
        {
            _componentsMap = new Dictionary<Type, int>();
            _components = new List<BooComponent>();
        }

        var type = typeof(T);
        T? component = GetComponent<T>();

        if (component != null) return component;

        component = (T)Activator.CreateInstance(type)!;
        _components.Add(component);
        _componentsMap.Add(type, _components.Count - 1);
        component.Parent = this;
        component.OnAttached(this);

        return component;
    }

    public void RemoveBehavior<T>() where T : BooComponent
    {
        if (_componentsMap == null || _components == null)
        {
            throw new InvalidOperationException();
        }

        var type = typeof(T);
        int index = _componentsMap[type];
        _components.RemoveAt(index);
        _componentsMap.Remove(type);
    }

    public T? GetComponent<T>() where T : BooComponent
    {
        if (_componentsMap == null || _components == null)
        {
            throw new InvalidOperationException();
        }

        if (_componentsMap.TryGetValue(typeof(T), out var index))
        {
            return (T)_components[index];
        }

        return null;
    }

    internal virtual void InternalProcess(GameTime time)
    {
        Process(time);

        if (_components == null) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Update(time);
            }
        }
    }

    public abstract void Process(GameTime time);

    public abstract void Draw(BooCanvas canvas);

    private Dictionary<Type, int>? _componentsMap;
    private List<BooComponent>? _components;
}