using System.Numerics;
using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class BooCanvasView
{
    public RectF Viewport { get; set; }

    public Matrix4x4 Projection { get; set; }
    
    public RenderTarget? RenderTarget { get; set; }
}

public class BooCanvas
{
    private readonly DynamicQuadMesh _mesh;
    private Texture2D? _currentTexture;
    private readonly ShaderProgram _defaultShader;
    private ShaderProgram _currentShader;
    private int _drawCalls;
    private int _maxDrawCalls;
    private ushort _renderPass;
    private BooCanvasView? _currentActiveView;

    public BooCanvas()
    {
        _mesh = new DynamicQuadMesh();
        _defaultShader = BooContent.GetEmbedded<ShaderProgram>("canvas2d");
        _currentShader = _defaultShader;

        for (int i = 0; i < 2; i++)
        {
            BooGraphics.SetRenderOrderMode(i, RenderOrderMode.Sequential);
        }
    }

    internal void Begin()
    {
        _currentShader = _defaultShader;

        _renderPass = 0;

        SetDefaultView();
        
        BooGraphics.Clear(0, Color.CornflowerBlue);

        _mesh.Reset();
    }

    private void SetDefaultView()
    {
        var view = Matrix4x4.Identity;
        var proj = Matrix4x4.CreateOrthographicOffCenter(0, BooGraphics.BackbufferWidth, BooGraphics.BackbufferHeight,
            0,
            -1.0f, 1.0f);
        
        BooGraphics.SetViewRect(0);
        BooGraphics.SetViewTransform(0, view, proj);
        BooGraphics.SetRenderTarget();
    }

    public void BeginView(BooCanvasView view)
    {
        if (_currentActiveView != null)
        {
            throw new BooException("Can't nest BeginViewCalls");
        }
        
        _currentActiveView = view;
        
        _renderPass++;

        if (view.RenderTarget != null)
        {
            BooGraphics.SetRenderTarget(view.RenderTarget);
        }
        
        BooGraphics.SetViewRect(_renderPass, (int)view.Viewport.X, (int)view.Viewport.Y, (int)view.Viewport.Width,
            (int)view.Viewport.Height);
        
        var viewMatrix = Matrix4x4.Identity;
        BooGraphics.SetViewTransform(_renderPass, viewMatrix, view.Projection);
    }

    public void EndView()
    {
        if (_currentActiveView == null)
        {
            throw new BooException("Calling EndView without BeginView first.");
        }

        _currentActiveView = null;
        
        Flush();
        
        SetDefaultView();
    }

    public void Clear(Color color)
    {
        BooGraphics.Clear(0, color);
    }

    public void SetViewport(float x, float y, float w, float h)
    {
        Flush();

        BooGraphics.SetViewRect(_renderPass, (int)x, (int)y, (int)w, (int)h);
    }

    public void SetViewport()
    {
        Flush();

        BooGraphics.SetViewRect(_renderPass);
    }

    public void SetProjectionRect(float x, float y, float w, float h)
    {
        Flush();

        var view = Matrix4x4.Identity;
        var proj = Matrix4x4.CreateOrthographicOffCenter(x, x + w, y + h, y,
            -1.0f, 1.0f);
        BooGraphics.SetViewTransform(_renderPass, view, proj);
    }

    public void SetShader(ShaderProgram? shader = null)
    {
        if (_currentShader != shader)
        {
            Flush();
        }

        _currentShader = shader ?? _defaultShader;
    }

    public void Draw(Texture2D texture, ref Quad quad)
    {
        if (_currentTexture == null)
        {
            _currentTexture = texture;
        }
        else if (!_currentTexture.Equals(texture))
        {
            Flush();
            _currentTexture = texture;
        }

        if (_mesh.VertexCount + 4 > _mesh.MaxVertices)
        {
            Flush();
        }

        _mesh.PushQuad(ref quad);
    }

    internal void End()
    {
        Flush();
        _drawCalls = 0;
        BooGraphics.DebugTextWrite(4, 4, DebugColor.Red, DebugColor.White, $"Draw Calls: {_maxDrawCalls}");
    }

    private void Flush()
    {
        if (_mesh.VertexCount == 0)
        {
            return;
        }

        BooGraphics.SetState();

        _currentShader.SetTexture(0, _currentTexture!);

        _mesh.Submit();

        _mesh.Reset();

        BooGraphics.Render(_renderPass, _currentShader);

        _drawCalls++;

        if (_drawCalls > _maxDrawCalls)
        {
            _maxDrawCalls = _drawCalls;
        }
    }
}