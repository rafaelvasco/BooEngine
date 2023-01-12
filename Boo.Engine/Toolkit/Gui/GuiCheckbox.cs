using Boo.Common.Math;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class GuiCheckbox : GuiControl
{
    public bool Checked { get; set; }
    
    public GuiCheckbox(string id, BooGui gui, GuiControl? control = null) : base(id, gui, control)
    {
        Width = 25;
        Height = 25;
    }

    public override void OnMouseUp(int localMouseX, int localMouseY)
    {
        Checked = !Checked;
    }

    public override void Draw()
    {
        Gui.Theme.DrawCheckBox(this);
    }
    
}