namespace Boo.Engine.Toolkit;

public class DefaultGuiTheme : BooGuiTheme
{
    public override string AtlasAsset => "uiSheet";
    
    
    public override void DrawButton(GuiButton button)
    {
        Gui.AddToRenderList("panel", button.GlobalX, button.GlobalY);
    }

    public override void DrawCheckBox(GuiCheckbox checkbox)
    {
        Gui.AddToRenderList("circle", checkbox.GlobalX, checkbox.GlobalY);
    }

    public override void DrawControl(GuiControl control)
    {
        Gui.AddToRenderList("panel", control.GlobalX, control.GlobalY);
    }

    public override void DrawSlider(GuiSlider slider)
    {
    }

    public DefaultGuiTheme(BooGui gui) : base(gui)
    {
    }
}