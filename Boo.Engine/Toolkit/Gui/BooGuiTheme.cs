namespace Boo.Engine.Toolkit;

public abstract class BooGuiTheme
{
    public abstract string AtlasAsset { get; }

    protected BooGuiTheme(BooGui gui)
    {
        Gui = gui;
    }

    protected BooGui Gui;
    
    public abstract void DrawButton(GuiButton button);
    public abstract void DrawCheckBox(GuiCheckbox checkbox);
    public abstract void DrawControl(GuiControl control);
    public abstract void DrawSlider(GuiSlider slider);
}