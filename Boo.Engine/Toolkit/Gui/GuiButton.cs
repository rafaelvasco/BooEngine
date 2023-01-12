namespace Boo.Engine.Toolkit;

public class GuiButton : GuiControl
{
    public string? Label { get; set; }

    public override bool Process()
    {
        return false;
    }

    public override void Draw()
    {
        Gui.Theme.DrawButton(this);
    }

    public override void OnMouseDown(int localMouseX, int localMouseY)
    {
        Console.WriteLine("OnMouseDown");
    }

    public override void OnMouseUp(int localMouseX, int localMouseY)
    {
        Console.WriteLine("OnMouseUp");
    }

    public override void OnActiveRelease()
    {
        Console.WriteLine("OnActiveRelease");
    }

    public GuiButton(string id, BooGui gui, GuiControl? control = null) : base(id, gui, control)
    {
        Width = 100;
        Height = 30;
    }
}