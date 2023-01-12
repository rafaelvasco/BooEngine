using System.Numerics;
using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Content;
using Boo.Engine.Graphics;
using Boo.Engine.Input;

namespace Boo.Engine.Toolkit;

internal class ControlDepthComparer : IComparer<GuiControl>
{
    public int Compare(GuiControl? x, GuiControl? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return y.Depth.CompareTo(x.Depth);
    }
}

public class BooGui : BooNode
{
    public BooGuiTheme Theme { get; }

    public BooGui(string name) : base(name)
    {
        _renderList = new Quad[128];
        
        _uiAtlas = BooContent.GetEmbedded<BooSpriteAtlas>("uiSheet");

        _uiSurface = BooGraphics.CreateRenderTarget(BooEngine.Settings.WindowWidth, BooEngine.Settings.WindowHeight);

        _uiCanvasView = new BooCanvasView()
        {
            RenderTarget = _uiSurface,
            Projection = Matrix4x4.CreateOrthographicOffCenter(0f, _uiSurface.Width, _uiSurface.Height, 0f, -1.0f, 1.0f),
            Viewport = new RectF(0f, 0f, _uiSurface.Width, _uiSurface.Height)
        };
        
        _quad = new Quad(_uiSurface.Texture);
        
        _allControls = new List<GuiControl>();
        _controlMap = new Dictionary<string, int>();
        _controlComparer = new ControlDepthComparer();

        Theme = new DefaultGuiTheme(this);
    }

    public void SetTheme(BooGuiTheme theme)
    {
        
    }

    internal void RegisterControl(GuiControl control)
    {
        _allControls.Add(control);
        _controlMap.Add(control.Id, _allControls.Count-1);
        
        _allControls.Sort(_controlComparer);
    }

    public override RectF BoundingRect => new(0, 0, _uiSurface.Width, _uiSurface.Height);

    public void Refresh()
    {
        _dirty = true;
    }
    
    public GuiControl this[string id]
    {
        get
        {
            if (_controlMap.TryGetValue(id, out var index))
            {
                return _allControls[index];
            }

            throw new BooException($"Couldn't find control with id {id}");
        }
    }

    private bool ProcessControlState(GuiControl control, bool hovered, bool active)
    {
        bool dirty = control.Hovered != hovered || control.Active != active;

        if (!dirty) return false;
        
        int mouseX = (int)(BooInput.Mouse.X - X);
        int mouseY = (int)(BooInput.Mouse.Y - Y);

        switch (active)
        {
            case true when hovered:
                control.OnMouseDown(mouseX - control.GlobalX, mouseY - control.GlobalY);
                break;
            case false:
                if (control.Active)
                {
                    control.OnActiveRelease();

                    if (hovered)
                    {
                        control.TriggerClick();
                        control.OnMouseUp(mouseX - control.GlobalX, mouseY - control.GlobalY);
                    }
                }
                break;
        }

        control.Hovered = hovered;
        control.Active = active;

        if (hovered && _hovered != null && control != _hovered)
        {
            _hovered.Hovered = false;
        }
        
        if (active && _active != null && control != _active)
        {
            _active.Active = false;
        }
        
        _hovered = hovered ? control : null;

        _active = active ? control : null;

        return dirty;
    }

    public override void Process(GameTime time)
    {
        int mouseX = (int)(BooInput.Mouse.X - X);
        int mouseY = (int)(BooInput.Mouse.Y - Y);

        bool changedState = false;

        foreach (var control in _allControls)
        {
            changedState |= control.Process();
            
            if (_active != null)
            {
                break;
            }
            
            if (!control.Enabled)
            {
                continue;
            }
            
            var isHovered = control.BoundingRect.Contains(mouseX, mouseY);

            changedState |= ProcessControlState(control, isHovered, _active == control);
            
            if (isHovered)
            {
                break;
            }
        }

        if (BooInput.Mouse.ButtonPressed(MouseButton.Left))
        {
            if (_hovered != null)
            {
                changedState |= ProcessControlState(_hovered, true, true);
            }
        }
        else if (BooInput.Mouse.ButtonReleased(MouseButton.Left))
        {
            if (_active != null)
            {
                changedState |= ProcessControlState(_active, _hovered == _active, false);
            }
        }

        if (_active != null)
        {
            if (BooInput.Mouse.DeltaX != 0 || BooInput.Mouse.DeltaY != 0)
            {
                changedState |= _active.OnMouseMove(mouseX - _active.GlobalX, mouseY - _active.GlobalY);
            }
        }

        _dirty = changedState;
    }


    internal void AddToRenderList(string uiElement, float x, float y)
    {
        var quad = _uiAtlas[uiElement];
        quad.SetXY(x, y);

        _renderList[_renderListIndex++] = quad;
    }

    public override void Draw(BooCanvas canvas)
    {
        if (_dirty)
        {
            _dirty = false;

            _renderListIndex = 0;

            foreach (var control in _allControls)
            {
                control.Draw();
            }
            
            canvas.BeginView(_uiCanvasView);
            
            for (int i = 0; i < _renderListIndex; ++i)
            {
                var quad = _renderList[i];
                canvas.Draw(_uiAtlas.Texture, ref quad);
            }
            
            canvas.EndView();
        }
        
        canvas.Draw(_uiSurface.Texture, ref _quad);
    }

    private readonly BooCanvasView _uiCanvasView;
    
    private readonly BooSpriteAtlas _uiAtlas;
    private readonly RenderTarget _uiSurface;
    private Quad _quad;
    private readonly Quad[] _renderList;
    private int _renderListIndex;
    private readonly List<GuiControl> _allControls;
    private readonly Dictionary<string, int> _controlMap;
    private GuiControl? _hovered;
    private GuiControl? _active;
    private readonly ControlDepthComparer _controlComparer;
    private bool _dirty;
}