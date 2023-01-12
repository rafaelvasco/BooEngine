using Boo.Engine;
using Boo.Engine.Graphics;
using Boo.Engine.Input;
using Boo.Engine.Toolkit;

namespace Boo.Playground;

public class BooGuiDemo : BooScene
{
    private BooGui? gui;
    
    public override void Load()
    {
        gui = new BooGui("gui");

        var panel = new GuiControl("panel", gui)
        {
            Width = 320,
            Height = 240
        };

        panel.X = (int)(gui.BoundingRect.Width / 2 - panel.Width / 2.0f);
        panel.Y = (int)(gui.BoundingRect.Height / 2 - panel.Height / 2.0f);

        // var button = new GuiButton("button", gui, panel);
        //
        // button.X = panel.BoundingRect.Width / 2 - button.Width/2;
        // button.Y = panel.BoundingRect.Height / 2 - button.Height/2;
        //
        // var button2 = new GuiButton("button2", gui, panel)
        // {
        //     X = button.X,
        //     Y = button.Y + button.Height + 10
        // };
        //
        // button2.OnClick += (sender, args) => Console.WriteLine("Click");
        //
        // var checkbox = new GuiCheckbox("check", gui, panel)
        // {
        //     X = button2.X,
        //     Y = button2.Y + button2.Height + 10
        // };
        //
        // var slider = new GuiSlider("slider", gui, panel)
        // {
        //     X = checkbox.X,
        //     Y = checkbox.Y + checkbox.Height + 10,
        //     MinValue = 0,
        //     MaxValue = 10,
        //     Step = 2
        // };

        Add(gui);
    }

    public override void Update(GameTime time)
    {
        // var slider = gui?["slider"] as GuiSlider;
        //
        // if (BooInput.Keyboard.KeyPressed(Key.Space))
        // {
        //     if (slider != null)
        //     {
        //         slider.Value = slider.MaxValue / 2;
        //     }
        // }
        // else if (BooInput.Keyboard.KeyPressed(Key.Left))
        // {
        //     if (slider != null)
        //     {
        //         slider.Value = slider.MinValue;
        //     }
        // }
        // else if (BooInput.Keyboard.KeyPressed(Key.Right))
        // {
        //     if (slider != null)
        //     {
        //         slider.Value = slider.MaxValue;
        //     }
        // }
    }

    public override void Draw(BooCanvas canvas)
    {
        base.Draw(canvas);
        
        // var slider = gui?["slider"] as GuiSlider;
        //
        // BooGraphics.DebugTextWrite(4, 12, DebugColor.Black, DebugColor.White,
        //     $"ValueStep: {slider?.ValueStep}, PosStep: {slider?.PositionStep}");
    }
}