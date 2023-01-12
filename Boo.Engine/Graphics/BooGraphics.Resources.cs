using Boo.Native;

namespace Boo.Engine.Graphics;

public static unsafe partial class BooGraphics
{
    internal static void RegisterRenderResource(BooDisposable resource)
    {
        _renderResources.Add(resource);
    }

    internal static Texture2D CreateTexture(string id, Texture2DCreateInfo info, bool mutable = false)
    {
        info.ThrowIfInvalid();

        if (mutable)
        {
            var textureHandle = Bgfx.create_texture_2d(
                (ushort)info.Width, (ushort)info.Height,
                false, 1,
                Texture2DCreateInfo.Format, (ulong)Texture2DCreateInfo.Flags,
                null
            );

            var texture = new Texture2D(id, textureHandle, info.Width, info.Height, Texture2DCreateInfo.Flags);
        
            texture.SetData(info.Data);

            return texture;
        }
        else
        {
            var textureHandle = Bgfx.create_texture_2d(
                (ushort)info.Width, (ushort)info.Height,
                false, 1,
                Texture2DCreateInfo.Format, (ulong)Texture2DCreateInfo.Flags,
                Bgfx.make_ref(info.Data.Pin().Pointer, (uint)(info.Width * info.Height * info.BytesPerPixel))
            );
            
            var texture = new Texture2D(id, textureHandle, info.Width, info.Height, Texture2DCreateInfo.Flags);

            return texture;
        }
    }

    internal static ShaderProgram CreateShaderProgram(string id, ShaderProgramCreateInfo info)
    {
        info.ThrowIfInvalid();

        var vsMem = BgfxUtils.MakeRef(info.VertexShader);
        var fsMem = BgfxUtils.MakeRef(info.FragmentShader);

        var vs = Bgfx.create_shader(vsMem);
        var fs = Bgfx.create_shader(fsMem);

        var handle = Bgfx.create_program(vs, fs, true);

        var shaderSamples = new ShaderSampler[info.Samplers.Length];

        var samplesSpan = info.Samplers.Span;

        for (int i = 0; i < info.Samplers.Length; ++i)
        {
            var samplerHandle = Bgfx.create_uniform(samplesSpan[i], Bgfx.UniformType.Sampler, 1);
            shaderSamples[i] = new ShaderSampler(samplerHandle);
        }

        var shaderParameters = new ShaderParameter[info.Parameters.Length];

        var parametersSpan = info.Parameters.Span;

        for (int i = 0; i < info.Parameters.Length; ++i)
        {
            var parameterHandle = Bgfx.create_uniform(parametersSpan[i], Bgfx.UniformType.Vec4, 4);

            shaderParameters[i] = new ShaderParameter(parameterHandle, parametersSpan[i]);
        }

        var shaderProgram = new ShaderProgram(id, handle, shaderSamples, shaderParameters);

        return shaderProgram;
    }

    public static RenderTarget CreateRenderTarget(int width, int height)
    {
        var handle = Bgfx.create_frame_buffer((ushort)width, (ushort)height, Bgfx.TextureFormat.BGRA8, (ulong)Bgfx.SamplerFlags.Point);

        var renderTarget = new RenderTarget(handle, width, height, Bgfx.SamplerFlags.Point);
        
        RegisterRenderResource(renderTarget);

        return renderTarget;
    }

    internal static Bgfx.TextureHandle GetFrameBufferTexture(Bgfx.FrameBufferHandle handle, byte attachment)
    {
        return Bgfx.get_texture(handle, attachment);
    }
    
    private static readonly List<BooDisposable> _renderResources = new();
}