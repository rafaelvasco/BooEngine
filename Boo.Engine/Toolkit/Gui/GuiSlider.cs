using Boo.Common;
using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public class GuiSlider : GuiControl
{
    public int Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                
                ValidateValues();

                CalculatePositionFromValues();

                Gui.Refresh();
            }
        }
    }

    public int Step
    {
        get => _valueStep;
        set
        {
            if (_valueStep != value)
            {
                _valueStep = value;

                ValidateValues();

                RecalculatePositionStep();
                
                CalculatePositionFromValues();
                
                Gui.Refresh();
            }
        }
    }

    public int MinValue
    {
        get => _minValue;
        set
        {
            if (_minValue != value)
            {
                _minValue = value;

                _value = _minValue;
                
                ValidateValues();

                RecalculatePositionStep();
                
                CalculatePositionFromValues();

                Gui.Refresh();
            }
        }
    }

    public int MaxValue
    {
        get => _maxValue;
        set
        {
            if (_maxValue != value)
            {
                _maxValue = value;

                _value = _minValue;
                
                ValidateValues();
                
                RecalculatePositionStep();
                
                CalculatePositionFromValues();

                Gui.Refresh();
            }
        }
    }

    private void ValidateValues()
    {
        if (_maxValue < _minValue)
        {
            throw new BooException("Invalid Min and Max Values: Min must be lower than Max.");
        }

        if (_minValue + _valueStep > MaxValue)
        {
            throw new BooException("Invalid Step Value: Surpasses MaxValue - MinValue");
        }

        if (_value < MinValue || _value > MaxValue)
        {
            throw new Exception("Invalid Value: Must be between Min and Max");
        }
    }

    internal int ThumbPosition => _thumbPosition;

    internal int ThumbSize => _thumbSize;

    public int ValueStep => _valueStep;

    public int PositionStep => _positionStep;

    public GuiSlider(string id, BooGui gui, GuiControl? control = null) : base(id, gui, control)
    {
        Width = 300;
        _thumbSize = 30;
        
        _minPos = 0;
        _maxPos = Width - _thumbSize;
        
        Height = _thumbSize;
        MinValue = 0;
        MaxValue = 10;
        Step = 1;
    }

    private void RecalculatePositionStep()
    {
        float factor = (float)_valueStep / (MaxValue - MinValue);

        _positionStep = (int)((_maxPos - _minPos) * factor);
    }

    private bool MoveThumbTo(int position)
    {
        _thumbPosition = position - _thumbSize / 2;

        _thumbPosition = (int)Calc.Snap(_thumbPosition, _positionStep);

        if (_thumbPosition < _minPos)
        {
            _thumbPosition = _minPos;
        }

        if (_thumbPosition > _maxPos)
        {
            _thumbPosition = _maxPos;
        }

        if (_thumbPosition != _lastThumbPosition)
        {
            _lastThumbPosition = _thumbPosition;
            UpdateValue();
            return true;
        }

        return false;
    }

    public override void OnMouseDown(int localMouseX, int localMouseY)
    {
        MoveThumbTo(localMouseX);
    }

    public override bool OnMouseMove(int localMouseX, int localMouseY)
    {
        return MoveThumbTo(localMouseX);
    }

    private void UpdateValue()
    {
        float factor = (float)_thumbPosition / (_maxPos - _minPos);

        _value = (int)Calc.Snap((_maxValue - _minValue) * factor, _valueStep);

        _value = Calc.Clamp(_value, _minValue, _maxValue);

        Console.WriteLine($"Value: {_value}");
    }

    private void CalculatePositionFromValues()
    {
        float factor = (float)_value / (_maxValue - _minValue);

        _thumbPosition = (int)((_maxPos - _minPos) * factor);
    }

    public override void Draw()
    {
        Gui.Theme.DrawSlider(this);
    }

    private int _minPos;
    private int _maxPos;
    private int _positionStep;
    private int _thumbPosition;
    private int _lastThumbPosition;
    private int _thumbSize;

    private int _value;
    private int _minValue;
    private int _maxValue;
    private int _valueStep;
}