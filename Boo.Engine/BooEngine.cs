using System.Runtime;
using System.Text.Json;
using Boo.Common.Content;
using Boo.Engine.Content;
using Boo.Engine.Platform;
using Boo.Engine.Graphics;
using Boo.Engine.Input;
using Boo.Engine.Toolkit;

namespace Boo.Engine;

public class BooEngine : BooDisposable
{
    public static (int Width, int Height) WindowSize
    {
        get => BooPlatform.GetWindowSize();
        set
        {
            var (width, height) = BooPlatform.GetWindowSize();
            if (value.Width == width && value.Height == height)
            {
                return;
            }

            BooPlatform.SetWindowSize(value.Width, value.Height);
        }
    }

    public static string Title
    {
        get => BooPlatform.GetWindowTitle();
        set => BooPlatform.SetWindowTitle(value);
    }

    public static bool Resizable
    {
        get => (BooPlatform.GetWindowFlags() & BooPlatform.WindowFlags.Resizable) != 0;
        set => BooPlatform.SetWindowResizable(value);
    }

    public static bool Borderless
    {
        get => (BooPlatform.GetWindowFlags() & BooPlatform.WindowFlags.Borderless) != 0;
        set => BooPlatform.SetWindowBorderless(value);
    }

    public static bool Fullscreen
    {
        get => BooPlatform.IsFullscreen();
        set
        {
            if (BooPlatform.IsFullscreen() == value)
            {
                return;
            }

            BooPlatform.SetWindowFullscreen(value);
        }
    }

    public static bool ShowCursor
    {
        get => BooPlatform.CursorVisible();
        set => BooPlatform.ShowCursor(value);
    }

    public static BooGameLoop GameLoop => _instance!._gameLoop;

    public static BooSettings Settings => _instance!._settings;

    public BooEngine()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        _instance = this;

        _settings = ProcessBooSettings();

        BooPlatform.Init(_settings);
        
        BooGraphics.Init(_settings);

        _gameLoop = new BooGameLoop
        {
            TargetFrameRate = _settings.DesiredFrameRate
        };
        
        BooContent.Init();

        BooInput.Init();
        
        BooPlatform.OnQuit = Exit;
        BooPlatform.Minimized = () => { _gameLoop.IsActive = false; };
        BooPlatform.Restored = () => { _gameLoop.IsActive = true; };

        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }
    
    public void Run(BooScene mainScene) 
    {
        _gameLoop.RunningScene = mainScene;

        mainScene.Load();

        _gameLoop.Tick();

        BooPlatform.ShowWindow(true);

        while (_gameLoop.Running)
        {
            BooPlatform.ProcessEvents();
            BooInput.Update();
            _gameLoop.Tick();
            BooGraphics.Present();

#if DEBUG
            if (BooInput.Keyboard.KeyPressed(Key.Escape))
            {
                Exit();
            }   
#endif
        }
    }

    public static void Exit()
    {
        if (!_instance!._gameLoop.Running) return;
        _instance._gameLoop.Running = false;
        _instance._gameLoop.SuppressDrawForOneFrame();
    }

    protected override void Free()
    {
        Console.WriteLine("BooEngine is cleaning up...");
        
        BooContent.Free();
        
        BooGraphics.Terminate();
        
        BooPlatform.Shutdown();
    }

    private static void ShowExceptionMessage(Exception ex)
    {
        BooPlatform.ShowRuntimeError("Boo", $"An Error Occurred: {ex.Message}");
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ShowExceptionMessage((Exception)e.ExceptionObject);
    }

    private static BooSettings ProcessBooSettings()
    {
        (bool modified, BooSettings) CheckData(BooSettings settings)
        {
            bool wasModified = false;

            var title = settings.WindowTitle;
            var wWidth = settings.WindowWidth;
            var wHeight = settings.WindowHeight;

            if (wWidth <= 0)
            {
                wWidth = 640;
                wasModified = true;
            }

            if (wHeight <= 0)
            {
                wHeight = 360;
                wasModified = true;
            }

            if (!wasModified)
            {
                return (false, settings);
            }

            var modSettings = new BooSettings(new BooSettings.BooSettingsData
            {
                WindowTitle = title,
                WindowWidth = wWidth,
                WindowHeight = wHeight,
            });

            return (true, modSettings);
        }

        void Write(BooSettings settings)
        {
            var newJson = JsonSerializer.Serialize(settings, BooSettings.JsonSettings());

            var path = Path.Combine(ContentProperties.AssetsFolder, ContentProperties.GameSettingsFile);

            File.WriteAllText(path, newJson);
        }

        var settings = BooContent.LoadSettings();
        
        var (modified, newSettings) = CheckData(settings);

        if (modified)
        {
            Write(newSettings);
        }

        return settings;
    }

    private static BooEngine? _instance;

    private readonly BooSettings _settings;

    private readonly BooGameLoop _gameLoop;
}
