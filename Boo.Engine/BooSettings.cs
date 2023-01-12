using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Boo.Common.Graphics;

namespace Boo.Engine;

public enum WindowMode
{
    [EnumMember(Value = "windowed")]
    Windowed,

    [EnumMember(Value = "borderless")]
    Borderless,

    [EnumMember(Value = "fullscreen")]
    Fullscreen
}

public readonly struct BooSettings
{
    public struct BooSettingsData
    {
        public string WindowTitle { get; set; }

        public int WindowWidth { get; set; }

        public int WindowHeight { get; set; }

        public WindowMode WindowMode { get; set; }

        public bool IsMouseVisible { get; set; }

        public bool VSync { get; set; }

        public bool IsWindowResizable { get; set; }

        public int DesiredFrameRate { get; set; }

        public GraphicsApi GraphicsApi { get; set; }

        public BooSettingsData()
        {
            WindowWidth = 960;
            WindowHeight = 540;
            WindowTitle = "Boo";
            WindowMode = WindowMode.Windowed;
            IsMouseVisible = true;
            IsWindowResizable = false;
            DesiredFrameRate = 60;
            GraphicsApi = GraphicsApi.Auto;
            VSync = false;
        }
    }

    public static JsonSerializerOptions JsonSettings() => new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public readonly string WindowTitle;
    public readonly int WindowWidth;
    public readonly int WindowHeight;
    public readonly WindowMode WindowMode;
    public readonly bool IsWindowResizable;
    public readonly bool IsMouseVisible;
    public readonly int DesiredFrameRate;
    public readonly bool VSync;
    public readonly GraphicsApi GraphicsApi;

    public BooSettings(BooSettingsData settingsData)
    {
        WindowTitle = settingsData.WindowTitle;
        WindowWidth = settingsData.WindowWidth;
        WindowHeight = settingsData.WindowHeight;
        WindowMode = settingsData.WindowMode;
        IsWindowResizable = settingsData.IsWindowResizable;
        IsMouseVisible = settingsData.IsMouseVisible;
        VSync = settingsData.VSync;
        DesiredFrameRate = settingsData.DesiredFrameRate;
        GraphicsApi = settingsData.GraphicsApi;
    }

    public override string ToString()
    {
        return $"BooSettings[" +
			       $"WindowTitle: {WindowTitle}, WindowWidth: {WindowWidth}, WindowHeight: " +
			       $"{WindowHeight}, WindowMode: {WindowMode}, GraphicsApi: {GraphicsApi}, IsWindowResizable: " +
			       $"{IsWindowResizable}, IsMouseVisible: {IsMouseVisible}]";
    }
}
