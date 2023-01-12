using System.Runtime.InteropServices;
using Boo.Common;
using static SDL2.SDL;

namespace Boo.Engine.Platform;

public enum BooPlatformId
{
    Windows,
    Osx,
    Linux
}

public delegate void FileDropEvent(FileDropEventArgs args);

public struct FileDropEventArgs
{
    public FileDropEventArgs(string[] files)
    {
        Files = files;
    }

    /// <summary>
    /// The paths of dropped files
    /// </summary>
    public string[] Files { get; }
}

internal static partial class BooPlatform
{
    public static Action? OnQuit;
    public static Action<FileDropEventArgs>? OnFileDrop;

    public static BooPlatformId PlatformId { get; private set; }

    private static List<string>? _dropList;

    public static void Init(BooSettings settings)
    {
        Ensure64BitArchitecture();

        DetectRunningBooPlatform();

#if DEBUG

        if (PlatformId == BooPlatformId.Windows)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                SDL_SetHint(
                    SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING,
                    "1"
                );
            }
        }
        
#endif

        SDL_SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
        SDL_SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER | SDL_INIT_HAPTIC) < 0)
        {
            SDL_Quit();
            throw new ApplicationException("Failed to initialize SDL");
        }

        CreateWindow(settings);

        InitMouse();
        InitGamePad();
    }

    public static void Shutdown()
    {
        Console.WriteLine("Quitting game application...");
        DestroyWindow();
        SDL_Quit();
    }

    public static void ProcessEvents()
    {
        while (SDL_PollEvent(out SDL_Event evt) == 1)
        {
            switch (evt.type)
            {
                case SDL_EventType.SDL_KEYDOWN or SDL_EventType.SDL_KEYUP:
                    ProcessKeyEvent(evt);
                    break;

                case SDL_EventType.SDL_MOUSEMOTION
                    or SDL_EventType.SDL_MOUSEBUTTONDOWN
                    or SDL_EventType.SDL_MOUSEBUTTONUP
                    or SDL_EventType.SDL_MOUSEWHEEL:
                    ProcessMouseEvent(evt);
                    break;

                case SDL_EventType.SDL_WINDOWEVENT:
                    ProcessWindowEvent(evt);
                    break;

                case SDL_EventType.SDL_CONTROLLERDEVICEADDED
                    or SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                    ProcessGamePadEvent(evt);
                    break;

                case SDL_EventType.SDL_TEXTINPUT:
                    ProcessTextInputEvent(evt);
                    break;

                case SDL_EventType.SDL_DROPFILE:
                    ProcessDropFile(evt);
                    break;

                case SDL_EventType.SDL_DROPCOMPLETE:
                    CompleteDropFile(evt);
                    break;

                case SDL_EventType.SDL_QUIT:

                    OnQuit!.Invoke();
                    break;
            }
        }
    }

    private static void ProcessDropFile(SDL_Event evt)
    {
        if (evt.drop.windowID != _windowId)
        {
            return;
        }

        string path = UTF8_ToManaged(evt.drop.file, freePtr: true);

        _dropList ??= new List<string>();

        _dropList.Add(path);
    }

    private static void CompleteDropFile(SDL_Event evt)
    {
        if (evt.drop.windowID != _windowId || _dropList == null)
        {
            return;
        }

        if (_dropList.Count <= 0) return;
        
        OnFileDrop?.Invoke(new FileDropEventArgs(_dropList.ToArray()));

        _dropList.Clear();
    }

    public static double GetPerformanceFrequency()
    {
        return SDL_GetPerformanceFrequency();
    }

    public static double GetPerformanceCounter()
    {
        return SDL_GetPerformanceCounter();
    }

    public static void ShowRuntimeError(string title, string message)
    {
        SDL_ShowSimpleMessageBox(
            SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
            title,
            message,
            IntPtr.Zero
        );
    }

    private static void DetectRunningBooPlatform()
    {
        if (OperatingSystem.IsWindows())
        {
            PlatformId = BooPlatformId.Windows;
        }
        else if (OperatingSystem.IsMacOS())
        {
            PlatformId = BooPlatformId.Osx;
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            PlatformId = BooPlatformId.Linux;
        }
        else
        {
            throw new BooException("Unsupported BooPlatform");
        }
    }

    private static void Ensure64BitArchitecture()
    {
        var runtimeArchitecture = RuntimeInformation.OSArchitecture;
        if (runtimeArchitecture is Architecture.Arm or Architecture.X86)
        {
            throw new NotSupportedException("32-bit architecture is not supported.");
        }
    }
}
