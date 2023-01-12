using Boo.Common;
using Boo.Engine.Platform;

namespace Boo.Engine.Input;

public enum InputMapCheckMode
{
    Pressed,
    Released,
    Down
}

public static class BooInput
{
    private delegate bool BooInputMapGamepadDelegate(InputMapCheckMode checkMode, GamePadButtons button);

    private delegate bool BooInputMapKeyboardDelegate(InputMapCheckMode checkMode, Key key);

    private delegate bool BooInputMapMouseDelegate(InputMapCheckMode checkMode, MouseButton button);

    public static event FileDropEvent? OnFileDrop;

    static BooInput()
    {
        _controlButtonMappings = new Dictionary<string, int>();

        _inputMappingsGamepad = new Dictionary<int, BooInputMapGamepadDelegate>();

        _inputMappingsKeyboard = new Dictionary<int, BooInputMapKeyboardDelegate>();

        _inputMappingsMouse = new Dictionary<int, BooInputMapMouseDelegate>();
    }

    public static Keyboard Keyboard { get; private set; } = null!;

    public static Mouse Mouse { get; private set; } = null!;

    public static Gamepad Gamepad { get; private set; } = null!;

    public static void Map(string controlName, GamePadButtons button, GamePadIndex index = GamePadIndex.One)
    {
        _controlButtonMappings[controlName] = (int)button;

        if (!_inputMappingsGamepad.TryGetValue((int)button, out _))
        {
            _inputMappingsGamepad[(int)button] = (mode, buttons) =>
            {
                return mode switch
                {
                    InputMapCheckMode.Pressed => Gamepad.ButtonPressed(buttons, index),
                    InputMapCheckMode.Released => Gamepad.ButtonReleased(buttons, index),
                    InputMapCheckMode.Down => Gamepad.ButtonDown(buttons, index),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
            };
        }
    }

    public static void Map(string controlName, Key key)
    {
        _controlButtonMappings[controlName] = (int)key;
        
        if (!_inputMappingsKeyboard.TryGetValue((int)key, out _))
        {
            _inputMappingsKeyboard[(int)key] = (mode, keys) =>
            {
                return mode switch
                {
                    InputMapCheckMode.Pressed => Keyboard.KeyPressed(keys),
                    InputMapCheckMode.Released => Keyboard.KeyReleased(keys),
                    InputMapCheckMode.Down => Keyboard.KeyDown(keys),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
            };
        }
    }

    public static void Map(string controlName, MouseButton button)
    {
        _controlButtonMappings[controlName] = (int)button;
        
        if (!_inputMappingsMouse.TryGetValue((int)button, out _))
        {
            _inputMappingsMouse[(int)button] = (mode, buttons) =>
            {
                return mode switch
                {
                    InputMapCheckMode.Pressed => Mouse.ButtonPressed(buttons),
                    InputMapCheckMode.Released => Mouse.ButtonReleased(buttons),
                    InputMapCheckMode.Down => Mouse.ButtonDown(buttons),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
            };
        }
    }

    public static bool CheckControl(string key, InputMapCheckMode check = InputMapCheckMode.Down)
    {
        if (!_controlButtonMappings.TryGetValue(key, out var checkButton))
            throw new BooException("Control key not found");
        
        if (_inputMappingsGamepad.TryGetValue(checkButton, out var checkerGp))
        {
            return checkerGp(check, (GamePadButtons)checkButton);
        }

        if (_inputMappingsKeyboard.TryGetValue(checkButton, out var checkerKb))
        {
            return checkerKb(check, (Key)checkButton);
        }

        if (_inputMappingsMouse.TryGetValue(checkButton, out var checkerMs))
        {
            return checkerMs(check, (MouseButton)checkButton);
        }

        throw new BooException("Control key not found");
    }
    
    internal static void Init()
    {
        Keyboard = new Keyboard();
        Mouse = new Mouse();
        Gamepad = new Gamepad();

        BooPlatform.OnFileDrop = args => ProcessFileDrop(args);
        
        MapDefaultControls();
    }

    private static void MapDefaultControls()
    {
        Map("InputUp", GamePadButtons.DPadUp);
        Map("InputDown", GamePadButtons.DPadDown);
        Map("InputLeft", GamePadButtons.DPadLeft);
        Map("InputRight", GamePadButtons.DPadRight);
    }

    internal static void Update()
    {
        Keyboard.UpdateState();
        Mouse.UpdateState();
        Gamepad.UpdateState();
    }
    
    private static void ProcessFileDrop(FileDropEventArgs fileDropArgs) => OnFileDrop?.Invoke(fileDropArgs);

    private static Dictionary<int, BooInputMapGamepadDelegate> _inputMappingsGamepad;
    private static Dictionary<int, BooInputMapKeyboardDelegate> _inputMappingsKeyboard;
    private static Dictionary<int, BooInputMapMouseDelegate> _inputMappingsMouse;

    private static Dictionary<string, int> _controlButtonMappings;

}
