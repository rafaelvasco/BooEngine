using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public enum WaveMovementMode
{
    Horizontal,
    Vertical
}

public enum WaveMovementType
{
    Sine,
    Triangle,
    SawTooth,
    ReverseSawTooth,
    Square
}

public class WaveMovement : BooComponent
{
    /// <summary>
    /// Movement mode; <see cref="WaveMovementMode"/>
    /// </summary>
    public WaveMovementMode MovementMode { get; set; } = WaveMovementMode.Horizontal;

    /// <summary>
    /// What wave calculation function to use. <see cref="WaveMovementType"/>
    /// </summary>
    public WaveMovementType WaveType { get; set; } = WaveMovementType.Sine;

    /// <summary>
    /// The time in seconds for a complete cycle;
    /// </summary>
    public float Period { get; set; } = 4.0f;

    /// <summary>
    /// The maximum amount of delta on the movement;
    /// </summary>
    public float Magnitude { get; set; } = 50.0f;

    public override void OnAttached(BooNode parent)
    {
        _initialValue = MovementMode switch
        {
            WaveMovementMode.Horizontal => parent.X,
            WaveMovementMode.Vertical => parent.Y,
            _ => throw new ArgumentOutOfRangeException()
        };

        _lastValue = _initialValue;
    }

    private float CalculateWaveValue(float value)
    {
        value %= Calc.TWO_PI;
        switch (WaveType)
        {
            case WaveMovementType.Sine:
                return Calc.Sin(value);
            case WaveMovementType.Triangle:
                return value switch
                {
                    <= Calc.PI_OVER2 => value / Calc.PI_OVER2,
                    <= 3 * Calc.PI_OVER2 => 1 - 2 * (value - Calc.PI_OVER2) / Calc.PI,
                    _ => (value - 3 * Calc.PI_OVER2) / Calc.PI_OVER2 - 1
                };

            case WaveMovementType.SawTooth:
                return 2 * value / Calc.TWO_PI - 1;
            case WaveMovementType.ReverseSawTooth:
                return -2 * value / Calc.TWO_PI + 1;
            case WaveMovementType.Square:
                return value < Calc.PI ? -1 : 1;
            default:
                return 0;
        }
    }

    private void UpdateFromPhase(BooNode parent)
    {
        switch (MovementMode)
        {
            case WaveMovementMode.Horizontal:
                if (!Calc.ApproximatelyEqual(parent.X, _lastValue))
                {
                    _initialValue += parent.X - _lastValue;
                }

                parent.X = _initialValue + CalculateWaveValue(_i) * Magnitude;
                _lastValue = parent.X;

                break;
            case WaveMovementMode.Vertical:
                if (!Calc.ApproximatelyEqual(parent.Y, _lastValue))
                {
                    _initialValue += parent.Y - _lastValue;
                }

                parent.Y = _initialValue + CalculateWaveValue(_i) * Magnitude;
                _lastValue = parent.Y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Update(GameTime time)
    {
        if (Parent == null)
        {
            return;
        }

        float dt = (float)time.ElapsedGameTime.TotalSeconds;

        if (Period == 0)
        {
            _i = 0;
        }
        else
        {
            _i += (dt / Period * Calc.TWO_PI) % Calc.TWO_PI;
        }

        UpdateFromPhase(Parent);
    }

    private float _lastValue;
    private float _initialValue;
    private float _i;
}