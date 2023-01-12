using System.Numerics;
using Boo.Common.Math;
using Boo.Engine;
using Boo.Engine.Content;
using Boo.Engine.Graphics;
using Boo.Engine.Input;
using Boo.Engine.Toolkit;

namespace Boo.Playground;

public class Character : BooSprite
{
    private readonly TimelineAnimation _timelineAnimation;
    
    public void ResetTint()
    {
        ColorTint = Color.White;
    }

    public void CallBack1()
    {
        ColorTint = Color.Red;
    }

    public void CallBack2()
    {
        ColorTint = Color.Blue;
    }


    public Character(float x, float y) : base(BooContent.Get<Texture2D>("sprite_sheet_char"))
    {
        X = x;
        Y = y;

        var animationComponent = _timelineAnimation = AddComponent<TimelineAnimation>();

        var horizontalAnimation = animationComponent.AddAnimation("RunningHorizontal");

        animationComponent.SetAnimation("RunningHorizontal");

        horizontalAnimation
            .AddTrack<SpriteFrameAnimationTrack>()
            .SetKeyFrame(0, 0.5f, new Rect(0, 2 * 64, 64, 64))
            .SetKeyFrame(1, 0.5f, new Rect(0 * 64, 6 * 64, 64, 64))
            .SetKeyFrame(2, 0.5f, new Rect(1 * 64, 6 * 64, 64, 64))
            .SetKeyFrame(3, 0.5f, new Rect(2 * 64, 6 * 64, 64, 64))
            .SetKeyFrame(4, 0.5f, new Rect(3 * 64, 6 * 64, 64, 64))
            .SetKeyFrame(5, 0.5f, new Rect(4 * 64, 6 * 64, 64, 64))
            .SetKeyFrame(6, 0.5f, new Rect(5 * 64, 6 * 64, 64, 64))
            .AnimationMode = AnimationLoopMode.Loop;

       
        horizontalAnimation
            .AddTrack<PositionAnimationTrack>()
            .SetKeyFrame(0, durationSec: 0f, new Vector2(x, y));
        horizontalAnimation
            .AddTrack<CallMethodAnimationTrack>()
            .SetKeyFrame(0, 0f, "ResetTint");
        
        var verticalAnimation = animationComponent.AddAnimation("RunningVertical");

        verticalAnimation
            .AddTrack<SpriteFrameAnimationTrack>()
            .SetKeyFrame(0, 0.5f, new Rect(0, 0, 64, 64))
            .SetKeyFrame(1, 0.5f, new Rect(0 * 64, 4 * 64, 64, 64))
            .SetKeyFrame(2, 0.5f, new Rect(1 * 64, 4 * 64, 64, 64))
            .SetKeyFrame(3, 0.5f, new Rect(2 * 64, 4 * 64, 64, 64))
            .SetKeyFrame(4, 0.5f, new Rect(3 * 64, 4 * 64, 64, 64))
            .SetKeyFrame(5, 0.5f, new Rect(4 * 64, 4 * 64, 64, 64))
            .SetKeyFrame(6, 0.5f, new Rect(5 * 64, 4 * 64, 64, 64))
            .AnimationMode = AnimationLoopMode.Loop;

        var positionAnimationTrack = verticalAnimation.AddTrack<PositionAnimationTrack>();

        positionAnimationTrack.AnimationMode = AnimationLoopMode.Loop;
        positionAnimationTrack.UpdateMode = ValueUpdateMode.Interpolated;
        positionAnimationTrack.EaseMode = Ease.InOutCubic;
        positionAnimationTrack.InterpolateLastFrameToFirst = true;

        positionAnimationTrack.SetKeyFrame(0, 0.5f, new Vector2(X, Y));
        positionAnimationTrack.SetKeyFrame(1, 0.5f, new Vector2(X + 100, Y));
        positionAnimationTrack.SetKeyFrame(2, 0.5f, new Vector2(X + 100, Y + 100));
        positionAnimationTrack.SetKeyFrame(3, 0.5f, new Vector2(X, Y + 100));

        var methodCallAnimationTrack = verticalAnimation.AddTrack<CallMethodAnimationTrack>();

        methodCallAnimationTrack.AnimationMode = AnimationLoopMode.Loop;

        methodCallAnimationTrack.SetKeyFrame(0, 0.5f, "CallBack1");
        methodCallAnimationTrack.SetKeyFrame(1, 0.5f, "CallBack2");
    }

    public override void Process(GameTime time)
    {
        if (BooInput.Keyboard.KeyPressed(Key.H))
        {
            _timelineAnimation.SetAnimation("RunningHorizontal");
        }
        else if (BooInput.Keyboard.KeyPressed(Key.V))
        {
            _timelineAnimation.SetAnimation("RunningVertical");
        }
    }
}

public class BooDemo : BooScene
{
    public override void Draw(BooCanvas canvas)
    {
        
    }

    public override void Load()
    {
        var viewport = new BooViewport("viewport1")
        {
            Size = new Size(960, 540)
        };
        
        var viewport2 = new BooViewport("viewport2");
        
        var tileset = BooContent.Get<BooTileset>("tileset_numbers");
        
        var tilemap = new BooTileMap(tileset, new []
        {
            Calc.Pack3(0, 0, 0),
            Calc.Pack3(1, 1, 0),
            Calc.Pack3(2, 2, 0),
            Calc.Pack3(3, 3, 0),
            
            Calc.Pack3(4, 0, 1),
            Calc.Pack3(5, 1, 1),
            Calc.Pack3(6, 2, 1),
            Calc.Pack3(7, 3, 1),
            
            Calc.Pack3(8, 0, 2),
            Calc.Pack3(9, 1, 2),
            Calc.Pack3(10, 2, 2),
            Calc.Pack3(11, 3, 2),
            
            Calc.Pack3(12, 0, 3),
            Calc.Pack3(13, 1, 3),
            Calc.Pack3(14, 2, 3),
            Calc.Pack3(15, 3, 3),
         
        }, 16 * 4, 16 * 4);
        
        var sprite = new BooSprite(BooContent.Get<Texture2D>("party"))
        {
            X = viewport.BoundingRect.Width / 2.0f,
            Y = viewport.BoundingRect.Height / 2.0f,
            Opacity = 0.5f
        };

        var sprite2 = new BooSprite(BooContent.Get<Texture2D>("party"))
        {
            X = viewport2.BoundingRect.Width / 2.0f,
            Y = viewport2.BoundingRect.Height / 2.0f
        };

        // var waveMov = sprite.AddComponent<WaveMovement>();
        // waveMov.Magnitude = 64;
        // waveMov.MovementMode = WaveMovementMode.Horizontal;
        // waveMov.WaveType = WaveMovementType.Sine;
        // waveMov.Period = 1.0f;

        sprite.AddComponent<DirectionalMovement>();

        var character = new Character(viewport.BoundingRect.Width / 2.0f, viewport.BoundingRect.Height / 2.0f)
        {
            Size = new Size(256, 256)
        };

        viewport.AddChild(tilemap);
        viewport.AddChild(sprite);
        viewport.AddChild(character);

        viewport2.AddChild(sprite2);
    
        Add(viewport);
        Add(viewport2);
    }
}