using Boo.Engine.Input;

namespace Boo.Engine.Toolkit;

public class DragAndDrop : BooComponent
{
    private bool _isDragging;

    public override void Update(GameTime time)
    {
        if (Parent == null)
        {
            return;
        }

        if (!BooInput.Mouse.ButtonDown(MouseButton.Left))
        {
            _isDragging = false;
            return;
        }

        if (!_isDragging && !Parent.BoundingRect.Contains(BooInput.Mouse.X, BooInput.Mouse.Y))
        {
            return;
        }
        
        if (!_isDragging)
        {
            _isDragging = true;
        }
        else
        {
            float deltaX = BooInput.Mouse.DeltaX;
            float deltaY = BooInput.Mouse.DeltaY;

            if (deltaX != 0.0f || deltaY != 0.0f)
            {
                Parent.X += deltaX;
                Parent.Y += deltaY;
            }
        }
    }
}