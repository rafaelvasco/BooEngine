using Boo.Common.Math;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class GuiControl
{
    public event EventHandler? OnClick;

    public BooGui Gui { get; }

    public string Id { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int GlobalX => X + (Parent?.X ?? 0);

    public int GlobalY => Y + (Parent?.Y ?? 0);

    public int Width { get; set; }

    public int Height { get; set; }
    
    public Rect BoundingRect => new(GlobalX, GlobalY, Width, Height);

    public GuiControlState State
    {
        get
        {
            if (Active)
            {
                return GuiControlState.Active;
            }

            return Hovered ? GuiControlState.Hovered : GuiControlState.Idle;
        }
    }
    
    public bool Hovered { get; internal set; }

    public bool Active { get; internal set; }

    public bool Enabled { get; set; } = true;
    
    public int Depth { get; internal set; }

    public GuiControl? Parent
    {
        get => _parent;
        set
        {
            if (_parent != value)
            {
                _parent = value;
                RecalculateDepth(this);
            }
        }
    }
    
    public GuiControl(string id, BooGui gui, GuiControl? control = null)
    {
        Gui = gui;
        Id = id;
        Parent = control;
        gui.RegisterControl(this);
    }

    public virtual bool Process()
    {
        return false;
    }

    internal void TriggerClick()
    {
        OnClick?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseDown(int localMouseX, int localMouseY)
    {
    }

    public virtual void OnMouseUp(int localMouseX, int localMouseY)
    {
    }

    public virtual bool OnMouseMove(int localMouseX, int localMouseY)
    {
        return false;
    }

    public virtual void OnActiveRelease()
    {
    }

    public virtual void Draw()
    {
        Gui.Theme.DrawControl(this);
    }

    private void RecalculateDepth(GuiControl control)
    {
        if (control.Parent == null) return;
        Depth++;
        RecalculateDepth(control.Parent);
    }

    private GuiControl? _parent;
}