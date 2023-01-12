using System.Text.Json.Nodes;
using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Input;

namespace Boo.Engine.Toolkit;

public enum DirectionalMovementType
{
    EightDirections,
    FourDirections,
    UpAndDown,
    LeftAndRight
}

public class DirectionalMovement : BooComponent
{

    /// <summary>
    /// Max speed limit;
    /// </summary>
    public float MaxSpeed { get; set; } = 200.0f;

    /// <summary>
    /// How much to add to speed each frame;
    /// </summary>
    public float Acceleration { get; set; } = 600.0f;

    /// <summary>
    /// How much to remove from speed each frame;
    /// </summary>
    public float Decceleration { get; set; } = 0.5f;

    /// <summary>
    /// When using the controller, use the left analog thumbstick to move;
    /// </summary>
    public bool UseControllerAnalog { get; set; }

    /// <summary>
    /// Movement mode; <see cref="DirectionalMovementType"/>
    /// </summary>
    public DirectionalMovementType MovementType { get; set; } = DirectionalMovementType.FourDirections;

    public override void SetParametersFromDefinitionData(JsonObject data)
    {
        if (data.ContainsKey(nameof(MaxSpeed)))
        {
            MaxSpeed = data[nameof(MaxSpeed)]!.GetValue<float>();
        }

        if (data.ContainsKey(nameof(Acceleration)))
        {
            Acceleration = data[nameof(Acceleration)]!.GetValue<float>();
        }

        if (data.ContainsKey(nameof(Decceleration)))
        {
            Decceleration = data[nameof(Decceleration)]!.GetValue<float>();
        }

        if (data.ContainsKey(nameof(UseControllerAnalog)))
        {
            UseControllerAnalog = data[nameof(UseControllerAnalog)]!.GetValue<bool>();
        }

        if (data.ContainsKey(nameof(MovementType)))
        {
            var movTypeStr = data[nameof(MovementType)]!.GetValue<string>();

            try
            {
                MovementType = (DirectionalMovementType)Enum.Parse(typeof(DirectionalMovementType), movTypeStr);
            }
            catch (Exception e)
            {
                throw new BooException($"Could not parse DirectionalMovement component definition: {e.Message}", e);
            }
        }
    }

    public override void Update(GameTime time)
    {
        if (Parent == null)
        {
            return;
        }

        var dt = time.FrameDeltaSec;

        if (UseControllerAnalog)
        {
            var gamepadThumbVector = BooInput.Gamepad.Thumbsticks().Left;

            switch (MovementType)
            {
                case DirectionalMovementType.EightDirections:
                    _speedX += gamepadThumbVector.X * Acceleration * dt;
                    _speedY += -gamepadThumbVector.Y * Acceleration * dt;
                    break;
                case DirectionalMovementType.FourDirections:
                    if (Calc.Abs(gamepadThumbVector.X) > Calc.Abs(gamepadThumbVector.Y))
                    {
                        _speedX += gamepadThumbVector.X * Acceleration * dt;
                        _speedY = 0f;
                    }
                    else
                    {
                        _speedX = 0f;
                        _speedY += -gamepadThumbVector.Y * Acceleration * dt;
                    }
                    break;
                case DirectionalMovementType.UpAndDown:
                    _speedX = 0f;
                    _speedY += -gamepadThumbVector.Y * Acceleration * dt;
                    break;
                case DirectionalMovementType.LeftAndRight:
                    _speedX += gamepadThumbVector.X * Acceleration * dt;
                    _speedY = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            switch (MovementType)
            {
                case DirectionalMovementType.EightDirections:
                    if (BooInput.CheckControl("InputLeft"))
                    {
                        _speedX -= Acceleration * dt;
                    }
                    else if (BooInput.CheckControl("InputRight"))
                    {
                        _speedX += Acceleration * dt;
                    }
            
                    if (BooInput.CheckControl("InputUp"))
                    {
                        _speedY -= Acceleration * dt;
                    }
                    else if (BooInput.CheckControl("InputDown"))
                    {
                        _speedY += Acceleration * dt;
                    }
                    break;
                case DirectionalMovementType.FourDirections:
                    if (BooInput.CheckControl("InputLeft"))
                    {
                        _speedX -= Acceleration * dt;
                        _speedY = 0f;
                    }
                    else if (BooInput.CheckControl("InputRight"))
                    {
                        _speedX += Acceleration * dt;
                        _speedY = 0f;
                    }
            
                    else if (BooInput.CheckControl("InputUp"))
                    {
                        _speedX = 0f;
                        _speedY -= Acceleration * dt;
                    }
                    else if (BooInput.CheckControl("InputDown"))
                    {
                        _speedX = 0f;
                        _speedY += Acceleration * dt;
                    }
                    break;
                case DirectionalMovementType.UpAndDown:
                    if (BooInput.CheckControl("InputUp"))
                    {
                        _speedX = 0f;
                        _speedY -= Acceleration * dt;
                    }
                    else if (BooInput.CheckControl("InputDown"))
                    {
                        _speedX = 0f;
                        _speedY += Acceleration * dt;
                    }
                    break;
                case DirectionalMovementType.LeftAndRight:
                    if (BooInput.CheckControl("InputLeft"))
                    {
                        _speedX -= Acceleration * dt;
                        _speedY = 0f;
                    }
                    else if (BooInput.CheckControl("InputRight"))
                    {
                        _speedX += Acceleration * dt;
                        _speedY = 0f;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }

        _speedX = _speedX switch
        {
            > 0 when _speedX > MaxSpeed => MaxSpeed,
            < 0 when _speedX < -MaxSpeed => -MaxSpeed,
            _ => _speedX
        };
        
        _speedY = _speedY switch
        {
            > 0 when _speedY > MaxSpeed => MaxSpeed,
            < 0 when _speedY < -MaxSpeed => -MaxSpeed,
            _ => _speedY
        };

        Parent.X += _speedX;
        Parent.Y += _speedY;

        Parent.X = Calc.Round(Parent.X);
        Parent.Y = Calc.Round(Parent.Y);
        
        _speedX *= Decceleration;
        _speedY *= Decceleration;

        if (Calc.Abs(_speedX) < 1.0f)
        {
            _speedX = 0.0f;
        }
        
        if (Calc.Abs(_speedY) < 1.0f)
        {
            _speedY = 0.0f;
        }
    }

    private float _speedX;
    private float _speedY;
}