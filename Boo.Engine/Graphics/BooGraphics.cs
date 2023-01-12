using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Boo.Common;
using Boo.Common.Graphics;
using Boo.Common.Math;
using Boo.Engine.Platform;
using Boo.Native;

namespace Boo.Engine.Graphics;

/// <summary>
/// Specifies various debug options.
/// </summary>
[Flags]
public enum DebugFeatures
{
    /// <summary>
    /// Don't enable any debugging features.
    /// </summary>
    None = 0,

    /// <summary>
    /// Force wireframe rendering.
    /// </summary>
    Wireframe = 0x1,

    /// <summary>
    /// When set, all rendering calls are skipped. This is useful when profiling to
    /// find bottlenecks between the CPU and GPU.
    /// </summary>
    InfinitelyFastHardware = 0x2,

    /// <summary>
    /// Display internal statistics.
    /// </summary>
    DisplayStatistics = 0x4,

    /// <summary>
    /// Display debug text.
    /// </summary>
    DisplayText = 0x8,

    /// <summary>
    /// Enable the internal library performance profiler.
    /// </summary>
    Profiler = 0x10
}

/// <summary>
/// Specifies debug text colors.
/// </summary>
public enum DebugColor
{
    /// <summary>
    /// Black.
    /// </summary>
    Black,

    /// <summary>
    /// Blue.
    /// </summary>
    Blue,

    /// <summary>
    /// Green.
    /// </summary>
    Green,

    /// <summary>
    /// Cyan.
    /// </summary>
    Cyan,

    /// <summary>
    /// Red.
    /// </summary>
    Red,

    /// <summary>
    /// Magenta.
    /// </summary>
    Magenta,

    /// <summary>
    /// Brown.
    /// </summary>
    Brown,

    /// <summary>
    /// Light gray.
    /// </summary>
    LightGray,

    /// <summary>
    /// Dark gray.
    /// </summary>
    DarkGray,

    /// <summary>
    /// Light blue.
    /// </summary>
    LightBlue,

    /// <summary>
    /// Light green.
    /// </summary>
    LightGreen,

    /// <summary>
    /// Light cyan.
    /// </summary>
    LightCyan,

    /// <summary>
    /// Light red.
    /// </summary>
    LightRed,

    /// <summary>
    /// Light magenta.
    /// </summary>
    LightMagenta,

    /// <summary>
    /// Yellow.
    /// </summary>
    Yellow,

    /// <summary>
    /// White.
    /// </summary>
    White
}

public struct BooGraphicsChanges : IEquatable<BooGraphicsChanges>
{
    public int BackbufferWidth;
    public int BackbufferHeight;
    public bool VsyncEnabled;

    public bool Equals(BooGraphicsChanges other)
    {
        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BackbufferWidth, BackbufferHeight, VsyncEnabled);
    }

    public override bool Equals(object? obj)
    {
        return obj is BooGraphicsChanges changes && Equals(changes);
    }

    public static bool operator ==(BooGraphicsChanges left, BooGraphicsChanges right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BooGraphicsChanges left, BooGraphicsChanges right)
    {
        return !(left == right);
    }
}

public enum BlendMode : ulong
{
    Solid,
    Mask,
    Add,
    Alpha,
    AlphaPre,
    Multiply,
    Light,
    Invert
}

public enum DepthTest : ulong
{
    Never = Bgfx.StateFlags.DepthTestNever,
    Less = Bgfx.StateFlags.DepthTestLess,
    LessEqual = Bgfx.StateFlags.DepthTestLequal,
    Greater = Bgfx.StateFlags.DepthTestGreater,
    GreaterEqual = Bgfx.StateFlags.DepthTestGequal
}

public enum RenderOrderMode
{
    Sequential = Bgfx.ViewMode.Sequential,
    DepthAscending = Bgfx.ViewMode.DepthAscending,
    DepthDescending = Bgfx.ViewMode.DepthDescending
}

public struct BooGraphicsState
{
    internal Bgfx.StateFlags StateFlags;

    public static ref BooGraphicsState Default => ref _default;

    private static BooGraphicsState _default;

    static BooGraphicsState()
    {
        _default = new BooGraphicsState(BlendMode.AlphaPre, DepthTest.LessEqual);
    }

    private const Bgfx.StateFlags _baseStateFlags = Bgfx.StateFlags.WriteRgb | Bgfx.StateFlags.WriteA |
                                                    Bgfx.StateFlags.WriteZ |
                                                    Bgfx.StateFlags.Msaa;


    public BooGraphicsState(BlendMode blend, DepthTest depthTest)
    {
        StateFlags = _baseStateFlags;
        StateFlags = ApplyStates(blend, depthTest);
    }

    private static Bgfx.StateFlags ApplyStates(BlendMode mode, DepthTest depthTest)
    {
        Bgfx.StateFlags blendState = mode switch
        {
            BlendMode.Solid => 0x0,
            BlendMode.Mask => Bgfx.StateFlags.BlendAlphaToCoverage,
            BlendMode.Add => BgfxUtils.STATE_BLEND_FUNC_SEPARATE(Bgfx.StateFlags.BlendSrcAlpha,
                Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendOne),
            BlendMode.Alpha => BgfxUtils.STATE_BLEND_FUNC_SEPARATE(Bgfx.StateFlags.BlendSrcAlpha,
                Bgfx.StateFlags.BlendInvSrcAlpha, Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendInvSrcAlpha),
            BlendMode.AlphaPre =>
                BgfxUtils.STATE_BLEND_FUNC(Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendInvSrcAlpha),
            BlendMode.Multiply => BgfxUtils.STATE_BLEND_FUNC(Bgfx.StateFlags.BlendDstColor, Bgfx.StateFlags.BlendZero),
            BlendMode.Light => BgfxUtils.STATE_BLEND_FUNC_SEPARATE(Bgfx.StateFlags.BlendDstColor,
                Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendZero, Bgfx.StateFlags.BlendOne),
            BlendMode.Invert => BgfxUtils.STATE_BLEND_FUNC(Bgfx.StateFlags.BlendInvDstColor,
                Bgfx.StateFlags.BlendInvSrcColor),
            _ => BgfxUtils.STATE_BLEND_FUNC(Bgfx.StateFlags.BlendOne, Bgfx.StateFlags.BlendInvSrcAlpha)
        };

        return _baseStateFlags | blendState | (Bgfx.StateFlags)depthTest;
    }
}

public static unsafe partial class BooGraphics
{
    public static int BackbufferWidth
    {
        get => _backbufferWidth;
        private set => _backbufferWidth = value;
    }

    public static int BackbufferHeight
    {
        get => _backbufferHeight;
        private set => _backbufferHeight = value;
    }

    public static bool VsyncEnabled
    {
        get => (ResetFlags & Bgfx.ResetFlags.Vsync) == Bgfx.ResetFlags.Vsync;
        private set => ResetFlags |= value ? Bgfx.ResetFlags.Vsync : Bgfx.ResetFlags.None;
    }

    internal static Bgfx.ResetFlags ResetFlags { get; private set; }

    internal static Bgfx.TextureFormat BackbufferFormat { get; private set; }

    public static GraphicsApi GraphicsApi => QuerySelectedGraphicsApi();

    internal static void Init(BooSettings settings)
    {
        InitializeGraphicsDriver(settings);

        if (QuerySelectedGraphicsApi() == GraphicsApi.Auto)
        {
            throw new BooException("No valid Graphics API could be created for this application.");
        }

        var state = CurrentGraphicsChangesState();

        state.BackbufferWidth = settings.WindowWidth;
        state.BackbufferHeight = settings.WindowHeight;

        ApplyChanges(state);

        BooPlatform.WindowResized = size =>
        {
            var graphicsChanges = new BooGraphicsChanges
            {
                BackbufferWidth = size.Width,
                BackbufferHeight = size.Height
            };

            ApplyChanges(graphicsChanges);
        };

        Bgfx.set_debug((uint)(Bgfx.DebugFlags.Text));
    }

    private static BooGraphicsChanges CurrentGraphicsChangesState()
    {
        return new BooGraphicsChanges
        {
            BackbufferWidth = BackbufferWidth,
            BackbufferHeight = BackbufferHeight,
            VsyncEnabled = VsyncEnabled
        };
    }

    internal static void Terminate()
    {
        Console.WriteLine("Shutting down graphics device...");

        foreach (var disposable in _renderResources)
        {
            disposable.Dispose();
        }

        _renderResources.Clear();

        Bgfx.shutdown();
    }

    private static void ApplyChanges(BooGraphicsChanges changes)
    {
        var currentState = CurrentGraphicsChangesState();

        if (currentState.Equals(changes)) return;

        BackbufferWidth = changes.BackbufferWidth;
        BackbufferHeight = changes.BackbufferHeight;
        VsyncEnabled = changes.VsyncEnabled;

        Bgfx.reset((uint)BackbufferWidth, (uint)BackbufferHeight, (uint)ResetFlags, BackbufferFormat);
    }

    internal static void Present()
    {
        Bgfx.frame(false);
    }

    public static void SetViewRect(int renderPass, int x, int y, int w, int h)
    {
        Bgfx.set_view_rect((ushort)renderPass, (ushort)x, (ushort)y, (ushort)w, (ushort)h);
    }

    public static void SetViewRect(int renderPass)
    {
        Bgfx.set_view_rect((ushort)renderPass, 0, 0, (ushort)_backbufferWidth, (ushort)_backbufferHeight);
    }

    public static void Clear(int renderPass, Color color)
    {
        Bgfx.set_view_clear((ushort)renderPass, (ushort)(Bgfx.ClearFlags.Color | Bgfx.ClearFlags.Depth), color.Rgba, 1.0f, 0);
        
        ClearDebugText();
        Bgfx.touch((ushort)renderPass);
    }

    public static void SetState(BooGraphicsState state)
    {
        Bgfx.set_state((ulong)(state.StateFlags),
            (uint)(Bgfx.StateFlags.BlendFactor | Bgfx.StateFlags.BlendInvSrcAlpha));
    }

    public static void SetState()
    {
        Bgfx.set_state((ulong)BooGraphicsState.Default.StateFlags,
            (uint)(Bgfx.StateFlags.BlendFactor | Bgfx.StateFlags.BlendInvSrcAlpha));
    }

    public static void SetViewTransform(int renderPass, Matrix4x4 view, Matrix4x4 proj)
    {
        Bgfx.set_view_transform((ushort)renderPass, Unsafe.AsPointer(ref view.M11), Unsafe.AsPointer(ref proj.M11));
    }

    public static void SetRenderOrderMode(int renderPass, RenderOrderMode renderOrderMode)
    {
        Bgfx.set_view_mode((ushort)renderPass, (Bgfx.ViewMode)renderOrderMode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetVertexBuffer(VertexBuffer vbo)
        => SetVertexBuffer(vbo, 0, vbo.VertexCount);

    public static void SetVertexBuffer(VertexBuffer vbo, int startVertex, int vertexCount)
    {
        Bgfx.set_vertex_buffer(
            0, vbo.Handle,
            (uint)startVertex, (uint)vertexCount
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDynamicVertexBuffer(DynamicVertexBuffer vbo)
        => SetDynamicVertexBuffer(vbo, 0, vbo.VertexCount);

    public static void SetDynamicVertexBuffer(DynamicVertexBuffer vbo, int startVertex,
        int vertexCount)
    {
        Bgfx.set_dynamic_vertex_buffer(
            0, vbo.Handle,
            (uint)startVertex, (uint)vertexCount
        );
    }

    public static void SetTransientVertexBuffer(ref TransientVertexBuffer tvb, int numVertices)
    {
        fixed (Bgfx.TransientVertexBuffer* ptr = &tvb.Handle)
        {
            Bgfx.set_transient_vertex_buffer(0, ptr, 0, (uint)numVertices);
        }
    }

    public static void SetTransientIndexBuffer(ref TransientIndexBuffer tib, int firstIndex, int numIndices)
    {
        fixed (Bgfx.TransientIndexBuffer* ptr = &tib.Handle)
        {
            Bgfx.set_transient_index_buffer(ptr, (uint)firstIndex, (uint)numIndices);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetIndexBuffer(IndexBuffer ibo)
        => SetIndexBuffer(ibo, 0, ibo.IndexCount);

    public static void SetIndexBuffer(IndexBuffer ibo, int startIndex, int indexCount)
    {
        Bgfx.set_index_buffer(ibo.Handle, (uint)startIndex, (uint)indexCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDynamicIndexBuffer(DynamicIndexBuffer ibo)
        => SetDynamicIndexBuffer(ibo, 0, ibo.IndexCount);

    public static void SetDynamicIndexBuffer(DynamicIndexBuffer ibo, int startIndex, int indexCount)
    {
        Bgfx.set_dynamic_index_buffer(ibo.Handle, (uint)startIndex, (uint)indexCount);
    }

    /// <summary>
    /// Enables debugging features.
    /// </summary>
    /// <param name="features">The set of debug features to enable.</param>
    public static void SetDebugFeatures(DebugFeatures features)
    {
        Bgfx.set_debug((uint)features);
    }


    /// <summary>
    /// Clears the debug text buffer.
    /// </summary>
    /// <param name="color">The color with which to clear the background.</param>
    /// <param name="smallText"><c>true</c> to use a small font for debug output; <c>false</c> to use normal sized text.</param>
    public static void ClearDebugText(DebugColor color = DebugColor.Black, bool smallText = false)
    {
        var attr = (byte)((byte)color << 4);
        Bgfx.dbg_text_clear(attr, smallText);
    }

    /// <summary>
    /// Writes debug text to the screen.
    /// </summary>
    /// <param name="x">The X position, in cells.</param>
    /// <param name="y">The Y position, in cells.</param>
    /// <param name="foreColor">The foreground color of the text.</param>
    /// <param name="backColor">The background color of the text.</param>
    /// <param name="format">The format of the message.</param>
    /// <param name="args">The arguments with which to format the message.</param>
    public static void DebugTextWrite(int x, int y, DebugColor foreColor, DebugColor backColor, string format,
        params object[] args)
    {
        DebugTextWrite(x, y, foreColor, backColor, string.Format(CultureInfo.CurrentCulture, format, args));
    }

    /// <summary>
    /// Writes debug text to the screen.
    /// </summary>
    /// <param name="x">The X position, in cells.</param>
    /// <param name="y">The Y position, in cells.</param>
    /// <param name="foreColor">The foreground color of the text.</param>
    /// <param name="backColor">The background color of the text.</param>
    /// <param name="message">The message to write.</param>
    public static void DebugTextWrite(int x, int y, DebugColor foreColor, DebugColor backColor, string message)
    {
        var attr = (byte)(((byte)backColor << 4) | (byte)foreColor);
        Bgfx.dbg_text_printf((ushort)x, (ushort)y, attr, "%s", message);
    }

    /// <summary>
    /// Draws data directly into the debug text buffer.
    /// </summary>
    /// <param name="x">The X position, in cells.</param>
    /// <param name="y">The Y position, in cells.</param>
    /// <param name="width">The width of the image to draw.</param>
    /// <param name="height">The height of the image to draw.</param>
    /// <param name="data">The image data bytes.</param>
    /// <param name="pitch">The pitch of each line in the image data.</param>
    public static void DebugTextImage(int x, int y, int width, int height, byte[] data, int pitch)
    {
        fixed (byte* ptr = data)
            Bgfx.dbg_text_image((ushort)x, (ushort)y, (ushort)width, (ushort)height, ptr, (ushort)pitch);
    }

    public static void SetRenderTarget(RenderTarget? target = null)
    {
        Bgfx.set_view_frame_buffer(0, target?.Handle ?? BgfxUtils.FrameBufferNone);
    }

    public static void Render(int renderPass, ShaderProgram shader)
    {
        SubmitShaderProgram(shader);

        Bgfx.submit((ushort)renderPass, shader.Handle, 0, (byte)Bgfx.DiscardFlags.All);
    }

    private static void SubmitShaderProgram(ShaderProgram shader)
    {
        void SetTexture(ShaderSampler sampler)
        {
            if (sampler.Texture != null)
            {
                Bgfx.set_texture(0, sampler.Handle, sampler.Texture.Handle, (uint)sampler.Texture.Flags);
            }
            else
            {
                throw new BooException("Shader Sampler with Null Texture.");
            }
        }

        if (shader.TextureSlotIndex == 0)
        {
            var sampler = shader.Samplers[0];

            SetTexture(sampler);
        }
        else
        {
            for (int i = 0; i < shader.TextureSlotIndex; ++i)
            {
                SetTexture(shader.Samplers[i]);
            }
        }

        if (shader.Parameters.Length == 0)
        {
            return;
        }

        for (int i = 0; i < shader.Parameters.Length; ++i)
        {
            var p = shader.Parameters[i];

            if (p.Constant)
            {
                if (p.SubmitedOnce)
                {
                    continue;
                }

                p.SubmitedOnce = true;
            }

            var val = p.Value;

            Bgfx.set_uniform(shader.Parameters[i].Handle, &val, 4);
        }
    }

    private static void InitializeGraphicsDriver(BooSettings settings)
    {
        var init = InitializeBgfx(settings);

        if (!Bgfx.init(init))
            throw new BooException("Bgfx failed to initialize.");

        Marshal.FreeHGlobal((IntPtr)init);

        ResetFlags = (Bgfx.ResetFlags)init->resolution.reset;
        BackbufferFormat = init->resolution.format;
    }

    private static Bgfx.Init* InitializeBgfx(BooSettings settings)
    {
        var nativeSurfaceHandles = BooPlatform.GetNativeSurfacePointers();

        var platformInfo = new Bgfx.PlatformData()
        {
            nwh = nativeSurfaceHandles.NativeWindowHandle.ToPointer(),
            ndt = nativeSurfaceHandles.NativeDisplayType.GetValueOrDefault().ToPointer()
        };

        var init = (Bgfx.Init*)Marshal.AllocHGlobal(sizeof(Bgfx.Init)).ToPointer();
        Bgfx.init_ctor(init);

        var rendererType = ProcessChosenGraphicsApi(settings.GraphicsApi);

        init->vendorId = (ushort)Bgfx.PciIdFlags.None;
        init->type = rendererType;
        init->deviceId = 0;
        init->allocator = IntPtr.Zero;
        init->callback = IntPtr.Zero;
        init->capabilities = ulong.MaxValue;
#if DEBUG
        init->debug = 1;
        init->profile = 1;
#else
			init->debug = 0;
			init->profile = 0;
#endif
        init->resolution.width = (uint)settings.WindowWidth;
        init->resolution.height = (uint)settings.WindowHeight;
        init->resolution.format = Bgfx.TextureFormat.BGRA8;
        init->resolution.reset = (uint)(settings.VSync ? Bgfx.ResetFlags.Vsync : Bgfx.ResetFlags.None);
        init->platformData = platformInfo;

        init->limits.maxEncoders = 1;
        init->limits.transientIbSize = 0xFFFF;
        init->limits.transientVbSize = 0xFFFF;
        init->limits.minResourceCbSize = 0;

        return init;
    }

    private static Bgfx.RendererType ProcessChosenGraphicsApi(GraphicsApi api)
    {
        if (api == GraphicsApi.Auto)
        {
            return BooPlatform.PlatformId switch
            {
                BooPlatformId.Windows => Bgfx.RendererType.Direct3D11,
                BooPlatformId.Osx => Bgfx.RendererType.Metal,
                BooPlatformId.Linux => Bgfx.RendererType.Vulkan,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        switch (api)
        {
            case GraphicsApi.Direct3D11:
            case GraphicsApi.Direct3D12:
                if (BooPlatform.PlatformId != BooPlatformId.Windows)
                {
                    throw new BooException("Non windows platforms can't choose D3D Graphics Backend");
                }

                return api == GraphicsApi.Direct3D11 ? Bgfx.RendererType.Direct3D11 : Bgfx.RendererType.Direct3D12;

            case GraphicsApi.Vulkan:
                return Bgfx.RendererType.Vulkan;
            case GraphicsApi.Metal:
                if (BooPlatform.PlatformId != BooPlatformId.Osx)
                {
                    throw new BooException("Non Mac platforms can't choose Metal Graphics Backend");
                }

                return Bgfx.RendererType.Metal;
            case GraphicsApi.OpenGl:
                return Bgfx.RendererType.OpenGL;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
    }

    private static GraphicsApi QuerySelectedGraphicsApi()
    {
        return Bgfx.get_renderer_type() switch
        {
            Bgfx.RendererType.Direct3D11 => GraphicsApi.Direct3D11,
            Bgfx.RendererType.Direct3D12 => GraphicsApi.Direct3D12,
            Bgfx.RendererType.Vulkan => GraphicsApi.Vulkan,
            Bgfx.RendererType.Metal => GraphicsApi.Metal,
            Bgfx.RendererType.OpenGL => GraphicsApi.OpenGl,
            _ => GraphicsApi.Auto
        };
    }

    private static int _backbufferWidth;
    private static int _backbufferHeight;
}