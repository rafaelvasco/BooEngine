using System.Numerics;
using Boo.Common.Math;
using Boo.Engine.Platform;

namespace Boo.Engine.Input;

/// <summary>
/// Defines the buttons on gamepad.
/// </summary>
[Flags]
public enum GamePadButtons
{
    /// <summary>
    /// Directional pad up.
    /// </summary>
    DPadUp = 0x00000001,
    /// <summary>
    /// Directional pad down.
    /// </summary>
    DPadDown = 0x00000002,
    /// <summary>
    /// Directional pad left.
    /// </summary>
    DPadLeft = 0x00000004,
    /// <summary>
    /// Directional pad right.
    /// </summary>
    DPadRight = 0x00000008,
    /// <summary>
    /// START button.
    /// </summary>
    Start = 0x00000010,
    /// <summary>
    /// BACK button.
    /// </summary>
    Back = 0x00000020,
    /// <summary>
    /// Left stick button (pressing the left stick).
    /// </summary>
    LeftStick = 0x00000040,
    /// <summary>
    /// Right stick button (pressing the right stick).
    /// </summary>
    RightStick = 0x00000080,
    /// <summary>
    /// Left bumper (shoulder) button.
    /// </summary>
    LeftShoulder = 0x00000100,
    /// <summary>
    /// Right bumper (shoulder) button.
    /// </summary>
    RightShoulder = 0x00000200,
    /// <summary>
    /// Big button.
    /// </summary>
    BigButton = 0x00000800,
    /// <summary>
    /// A button.
    /// </summary>
    A = 0x00001000,
    /// <summary>
    /// B button.
    /// </summary>
    B = 0x00002000,
    /// <summary>
    /// X button.
    /// </summary>
    X = 0x00004000,
    /// <summary>
    /// Y button.
    /// </summary>
    Y = 0x00008000,
    /// <summary>
    /// Left stick is towards the left.
    /// </summary>
    LeftThumbstickLeft = 0x00200000,
    /// <summary>
    /// Right trigger.
    /// </summary>
    RightTrigger = 0x00400000,
    /// <summary>
    /// Left trigger.
    /// </summary>
    LeftTrigger = 0x00800000,
    /// <summary>
    /// Right stick is towards up.
    /// </summary>
    RightThumbstickUp = 0x01000000,
    /// <summary>
    /// Right stick is towards down.
    /// </summary>
    RightThumbstickDown = 0x02000000,
    /// <summary>
    /// Right stick is towards the right.
    /// </summary>
    RightThumbstickRight = 0x04000000,
    /// <summary>
    /// Right stick is towards the left.
    /// </summary>
    RightThumbstickLeft = 0x08000000,
    /// <summary>
    /// Left stick is towards up.
    /// </summary>
    LeftThumbstickUp = 0x10000000,
    /// <summary>
    /// Left stick is towards down.
    /// </summary>
    LeftThumbstickDown = 0x20000000,
    /// <summary>
    /// Left stick is towards the right.
    /// </summary>
    LeftThumbstickRight = 0x40000000
}

public struct GamePadCapabilities
{
    public bool IsConnected
    {
        get;
        internal set;
    }

    public bool HasAButton
    {
        get;
        internal set;
    }

    public bool HasBackButton
    {
        get;
        internal set;
    }

    public bool HasBButton
    {
        get;
        internal set;
    }

    public bool HasDPadDownButton
    {
        get;
        internal set;
    }

    public bool HasDPadLeftButton
    {
        get;
        internal set;
    }

    public bool HasDPadRightButton
    {
        get;
        internal set;
    }

    public bool HasDPadUpButton
    {
        get;
        internal set;
    }

    public bool HasLeftShoulderButton
    {
        get;
        internal set;
    }

    public bool HasLeftStickButton
    {
        get;
        internal set;
    }

    public bool HasRightShoulderButton
    {
        get;
        internal set;
    }

    public bool HasRightStickButton
    {
        get;
        internal set;
    }

    public bool HasStartButton
    {
        get;
        internal set;
    }

    public bool HasXButton
    {
        get;
        internal set;
    }

    public bool HasYButton
    {
        get;
        internal set;
    }

    public bool HasBigButton
    {
        get;
        internal set;
    }

    public bool HasLeftXThumbStick
    {
        get;
        internal set;
    }

    public bool HasLeftYThumbStick
    {
        get;
        internal set;
    }

    public bool HasRightXThumbStick
    {
        get;
        internal set;
    }

    public bool HasRightYThumbStick
    {
        get;
        internal set;
    }

    public bool HasLeftTrigger
    {
        get;
        internal set;
    }

    public bool HasRightTrigger
    {
        get;
        internal set;
    }

    public bool HasLeftVibrationMotor
    {
        get;
        internal set;
    }

    public bool HasRightVibrationMotor
    {
        get;
        internal set;
    }

    public bool HasVoiceSupport
    {
        get;
        internal set;
    }

    public GamePadType GamePadType
    {
        get;
        internal set;
    }
}

/// <summary>
/// Specifies a type of dead zone processing to apply to the controllers analog sticks when
/// calling GetState.
/// </summary>
public enum GamePadDeadZone
{
    /// <summary>
    /// The values of each stick are not processed and are returned by GetState as "raw" values.
    /// This is best if you intend to implement your own dead zone processing.
    /// </summary>
    None,

    /// <summary>
    /// The X and Y positions of each stick are compared against the dead zone independently.
    /// This setting is the default when calling GetState.
    /// </summary>
    IndependentAxes,

    /// <summary>
    /// The combined X and Y position of each stick is compared to the dead zone. This provides
    /// better control than IndependentAxes when the stick is used as a two-dimensional control
    /// surface, such as when controlling a character's view in a first-person game.
    /// </summary>
    Circular
}

public enum GamePadIndex
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3
}

public enum GamePadType
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown,
    /// <summary>
    /// GamePad is the XBOX controller.
    /// </summary>
    GamePad,
    /// <summary>
    /// GamePad is a wheel.
    /// </summary>
    Wheel,
    /// <summary>
    /// GamePad is an arcade stick.
    /// </summary>
    ArcadeStick,
    /// <summary>
    /// GamePad is a flight stick.
    /// </summary>
    FlightStick,
    /// <summary>
    /// GamePad is a dance pad.
    /// </summary>
    DancePad,
    /// <summary>
    /// GamePad is a guitar.
    /// </summary>
    Guitar,
    /// <summary>
    /// GamePad is an alternate guitar.
    /// </summary>
    AlternateGuitar,
    /// <summary>
    /// GamePad is a drum kit.
    /// </summary>
    DrumKit,
    /// <summary>
    /// GamePad is a big button pad.
    /// </summary>
    BigButtonPad,
}

public struct GamePadThumbSticks
{
    public Vector2 Left => _left;

    public Vector2 Right => _right;

    private Vector2 _left;
    private Vector2 _right;

    public GamePadThumbSticks(Vector2 leftPosition, Vector2 rightPosition)
    {
        _left = leftPosition;
        _right = rightPosition;
        ApplySquareClamp();
    }

    internal GamePadThumbSticks(
        Vector2 leftPosition,
        Vector2 rightPosition,
        GamePadDeadZone deadZoneMode
    )
    {
        _left = leftPosition;
        _right = rightPosition;
        ApplyDeadZone(deadZoneMode);
        if (deadZoneMode == GamePadDeadZone.Circular)
        {
            ApplyCircularClamp();
        }
        else
        {
            ApplySquareClamp();
        }
    }

    private void ApplyDeadZone(GamePadDeadZone dz)
    {
        switch (dz)
        {
            case GamePadDeadZone.None:
                break;
            case GamePadDeadZone.IndependentAxes:
                _left.X = Gamepad.ExcludeAxisDeadZone(_left.X, Gamepad.LeftDeadZone);
                _left.Y = Gamepad.ExcludeAxisDeadZone(_left.Y, Gamepad.LeftDeadZone);
                _right.X = Gamepad.ExcludeAxisDeadZone(_right.X, Gamepad.RightDeadZone);
                _right.Y = Gamepad.ExcludeAxisDeadZone(_right.Y, Gamepad.RightDeadZone);
                break;
            case GamePadDeadZone.Circular:
                _left = ExcludeCircularDeadZone(_left, Gamepad.LeftDeadZone);
                _right = ExcludeCircularDeadZone(_right, Gamepad.RightDeadZone);
                break;
        }
    }

    private void ApplySquareClamp()
    {
        _left.X = Calc.Clamp(_left.X, -1.0f, 1.0f);
        _left.Y = Calc.Clamp(_left.Y, -1.0f, 1.0f);
        _right.X = Calc.Clamp(_right.X, -1.0f, 1.0f);
        _right.Y = Calc.Clamp(_right.Y, -1.0f, 1.0f);
    }

    private void ApplyCircularClamp()
    {
        if (_left.LengthSquared() > 1.0f)
        {
            _left = _left.Normalize();
        }
        if (_right.LengthSquared() > 1.0f)
        {
            _right = _right.Normalize();
        }
    }

    private static Vector2 ExcludeCircularDeadZone(Vector2 value, float deadZone)
    {
        float originalLength = value.Length();
        if (originalLength <= deadZone)
        {
            return Vector2.Zero;
        }
        float newLength = (originalLength - deadZone) / (1.0f - deadZone);
        return value * (newLength / originalLength);
    }

   

    /// <summary>
    /// Determines whether two specified instances of <see cref="GamePadThumbSticks"/>
    /// are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// True if <paramref name="left"/> and <paramref name="right"/> are equal;
    /// otherwise, false.
    /// </returns>
    public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
    {
        return (left._left == right._left) && (left._right == right._right);
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="GamePadThumbSticks"/>
    /// are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// True if <paramref name="left"/> and <paramref name="right"/> are not equal;
    /// otherwise, false.
    /// </returns>
    public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">An object to compare to this instance.</param>
    /// <returns>
    /// True if <paramref name="obj"/> is a <see cref="GamePadThumbSticks"/> and has the
    /// same value as this instance; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return (obj is GamePadThumbSticks sticks) && (this == sticks);
    }

    public override int GetHashCode()
    {
        return this.Left.GetHashCode() + 37 * this.Right.GetHashCode();
    }

}

public struct GamePadTriggers
{
    public float Left { get; }

    public float Right { get; }

    public GamePadTriggers(float leftTrigger, float rightTrigger)
    {
        Left = Calc.Clamp(leftTrigger, 0.0f, 1.0f);
        Right = Calc.Clamp(rightTrigger, 0.0f, 1.0f);
    }

    internal GamePadTriggers(
        float leftTrigger,
        float rightTrigger,
        GamePadDeadZone deadZoneMode
    )
    {
        /* XNA applies dead zones before rounding/clamping values.
		 * The public constructor does not allow this because the
		 * dead zone must be known first.
		 */
        if (deadZoneMode == GamePadDeadZone.None)
        {
            Left = Calc.Clamp(leftTrigger, 0.0f, 1.0f);
            Right = Calc.Clamp(rightTrigger, 0.0f, 1.0f);
        }
        else
        {
            Left = Calc.Clamp(
                Gamepad.ExcludeAxisDeadZone(
                    leftTrigger,
                    Gamepad.TriggerThreshold
                ),
                0.0f,
                1.0f
            );
            Right = Calc.Clamp(
                Gamepad.ExcludeAxisDeadZone(
                    rightTrigger,
                    Gamepad.TriggerThreshold
                ),
                0.0f,
                1.0f
            );
        }
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are
    /// equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// True if <paramref name="left"/> and <paramref name="right"/> are equal;
    /// otherwise, false.
    /// </returns>
    public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
    {
        return ((Calc.ApproximatelyEqual(left.Left, right.Left)) &&
                (Calc.ApproximatelyEqual(left.Right, right.Right)));
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are
    /// not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// True if <paramref name="left"/> and <paramref name="right"/> are not equal;
    /// otherwise, false.
    /// </returns>
    public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">An object to compare to this instance.</param>
    /// <returns>
    /// True if <paramref name="obj"/> is a <see cref="GamePadTriggers"/> and has the
    /// same value as this instance; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return (obj is GamePadTriggers triggers) && (this == triggers);
    }

    public override int GetHashCode()
    {
        return this.Left.GetHashCode() + this.Right.GetHashCode();
    }

}


/// <summary>
/// Represents specific information about the state of a controller,
/// including the current state of buttons and sticks.
/// </summary>
public struct GamePadState
{
    /// <summary>
    /// Indicates whether the controller is connected.
    /// </summary>
    public bool IsConnected
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the packet number associated with this state.
    /// </summary>
    public int PacketNumber
    {
        get;
        internal set;
    }

    /// <summary>
    /// Returns a structure that identifies which buttons on the controller
    /// are pressed.
    /// </summary>
    public GamePadButtons Buttons
    {
        get;
        internal set;
    }

    /// <summary>
    /// Returns a structure that indicates the position of the controller thumbsticks.
    /// </summary>
    public GamePadThumbSticks ThumbSticks
    {
        get;
        internal set;
    }

    /// <summary>
    /// Returns a structure that identifies the position of triggers on the controller.
    /// </summary>
    public GamePadTriggers Triggers
    {
        get;
        internal set;
    }

    public bool this[GamePadButtons button] => (Buttons & button) == button;


    /// <summary>
    /// Initializes a new instance of the GamePadState class using the specified
    /// GamePadThumbSticks, GamePadTriggers, GamePadButtons, and GamePadDPad.
    /// </summary>
    /// <param name="thumbSticks">Initial thumbstick state.</param>
    /// <param name="triggers">Initial trigger state.</param>
    /// <param name="buttons">Initial button state.</param>
    public GamePadState(
        GamePadThumbSticks thumbSticks,
        GamePadTriggers triggers,
        GamePadButtons buttons
    ) : this()
    {
        ThumbSticks = thumbSticks;
        Triggers = triggers;
        Buttons = buttons;
        IsConnected = true;
        PacketNumber = 0;
    }

    /// <summary>
    /// Determines whether two GamePadState instances are equal.
    /// </summary>
    /// <param name="left">Object on the left of the equal sign.</param>
    /// <param name="right">Object on the right of the equal sign.</param>
    public static bool operator ==(GamePadState left, GamePadState right)
    {
        return ((left.IsConnected == right.IsConnected) &&
                (left.PacketNumber == right.PacketNumber) &&
                (left.Buttons == right.Buttons) &&
                (left.ThumbSticks == right.ThumbSticks) &&
                (left.Triggers == right.Triggers));
    }

    /// <summary>
    /// Determines whether two GamePadState instances are not equal.
    /// </summary>
    /// <param name="left">Object on the left of the equal sign.</param>
    /// <param name="right">Object on the right of the equal sign.</param>
    public static bool operator !=(GamePadState left, GamePadState right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a value that indicates whether the current instance is equal to a
    /// specified object.
    /// </summary>
    /// <param name="obj">Object with which to make the comparison.</param>
    public override bool Equals(object? obj)
    {
        return (obj is GamePadState state) && (this == state);
    }

    public bool Equals(GamePadState other)
    {
        return IsConnected == other.IsConnected && PacketNumber == other.PacketNumber && Buttons == other.Buttons && ThumbSticks.Equals(other.ThumbSticks) && Triggers.Equals(other.Triggers);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsConnected, PacketNumber, (int)Buttons, ThumbSticks, Triggers);
    }
}

public class Gamepad
{
    private readonly GamePadState[] _gpState;
    private readonly GamePadState[] _lastGpState;
    

    /* Based on the XInput constants */
    internal const float LeftDeadZone = 7849.0f / 32768.0f;
    internal const float RightDeadZone = 8689.0f / 32768.0f;
    internal const float TriggerThreshold = 30.0f / 255.0f;

    public int ConnectedGamePads => BooPlatform.ConnectedGamePads;

    public static readonly int MaxCount = 4;

    public bool EnableGamepad { get; set; } = true;

    internal Gamepad()
    {
        _gpState = new GamePadState[MaxCount];
        _lastGpState = new GamePadState[MaxCount];

        //TODO:
        //var gamepad_mappings_text_file = Engine.Content.Get<TextFile>("gamecontrollerdb");

        //var gamepad_mappings_text_content = gamepad_mappings_text_file.JoinedText;

        //Platform.SetGamePadMappingsFile(gamepad_mappings_text_content);

        BooPlatform.PreLookForGamepads();

        if (ConnectedGamePads <= 0) return;
        
        for (int i = 0; i < ConnectedGamePads; i++)
        {
            _gpState[i] = GetState((GamePadIndex)i);
            _lastGpState[i] = _gpState[i];
        }
    }

    internal void UpdateState()
    {
        if (ConnectedGamePads == 1)
        {
            _lastGpState[0] = _gpState[0];
            _gpState[0] = GetState(GamePadIndex.One);
        }
        else
        {
            for (int i = 0; i < ConnectedGamePads; ++i)
            {
                _lastGpState[i] = _gpState[i];
                _gpState[i] = GetState((GamePadIndex)i);
            }
        }
    }

    public GamePadCapabilities GetInfo(GamePadIndex gamepadIndex)
    {
        return BooPlatform.GetGamePadCapabilities((int)gamepadIndex);
    }

    public bool ButtonPressed(GamePadButtons button, GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        int index = (int)gamePadIndex;
        return (_gpState[index].Buttons & button) != 0 && (_lastGpState[index].Buttons & button) == 0;
    }

    public bool ButtonDown(GamePadButtons button, GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        return (_gpState[(int)gamePadIndex].Buttons & button) != 0;
    }

    public bool ButtonReleased(GamePadButtons button, GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        int index = (int)gamePadIndex;
           
        return (_gpState[index].Buttons & button) == 0 && (_lastGpState[index].Buttons & button) != 0;
    }
    
    public GamePadTriggers Triggers(GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        return _gpState[(int)gamePadIndex].Triggers;
    }

    public GamePadThumbSticks Thumbsticks(GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        return _gpState[(int)gamePadIndex].ThumbSticks;
    }

    public bool IsConnected(GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        return _gpState[(int)gamePadIndex].IsConnected;
    }
    
    public GamePadState GetState(GamePadIndex gamepadIndex = GamePadIndex.One)
    {
        return BooPlatform.GetGamePadState(
            (int)gamepadIndex,
            GamePadDeadZone.IndependentAxes
        );
    }

    public bool ChangedState(GamePadIndex gamePadIndex = GamePadIndex.One)
    {
        int index = (int)gamePadIndex;
        return _gpState[index] != _lastGpState[index];
    }

    public GamePadState GetState(GamePadIndex gamepadIndex, GamePadDeadZone deadZoneMode)
    {
        return BooPlatform.GetGamePadState(
            (int)gamepadIndex,
            deadZoneMode
        );
    }

    public bool SetVibration(GamePadIndex gamepadIndex, float leftMotor, float rightMotor)
    {
        return BooPlatform.SetGamePadVibration(
            (int)gamepadIndex,
            leftMotor,
            rightMotor
        );
    }

    public string GetGuid(GamePadIndex gamepadIndex)
    {
        return BooPlatform.GetGamePadGuid((int)gamepadIndex);
    }

    public void SetLightbar(GamePadIndex gamepadIndex, Color color)
    {
        BooPlatform.SetGamePadLightBar((int)gamepadIndex, color);
    }

    internal static float ExcludeAxisDeadZone(float value, float deadZone)
    {
        if (value < -deadZone)
        {
            value += deadZone;
        }
        else if (value > deadZone)
        {
            value -= deadZone;
        }
        else
        {
            return 0.0f;
        }
        return value / (1.0f - deadZone);
    }
}