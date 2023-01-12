using System.Diagnostics;
using Boo.Common;
using Boo.Engine.Platform;
using Boo.Engine.Toolkit;
using Boo.Native;

namespace Boo.Engine;

public class BooGameLoop
{
    public bool Running { get; set; } = true;

    public BooScene? RunningScene { get; set; }

    public int TargetFrameRate
    {
        get => _targetFrameRate;
        set
        {
            if (value <= 0)
            {
                throw new BooException("The frame rate must be positive and non-zero");
            }

            _targetFrameRate = value;

            _targetElapsedTime = TimeSpan.FromTicks((long)(BooPlatform.GetPerformanceFrequency() / value));

            if (_targetElapsedTime > _maxElapsedTime)
            {
                throw new BooException("The frame rate resulting target elapsed time cannot exceed MaxElapsedTime");
            }
        }
    }

    public bool IsFixedTimeStep { get; set; } = true;

    public TimeSpan MaxElapsedTime
    {
        get => _maxElapsedTime;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("The time must be positive", default(Exception));
            }

            if (value < _targetElapsedTime)
            {
                throw new ArgumentOutOfRangeException("The time must be at least equal to TargetElapsedTime",
                    default(Exception));
            }

            _maxElapsedTime = value;
        }
    }

    public TimeSpan InactiveSleepTime
    {
        get => _inactiveSleepTime;
        set
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("The time must be positive.", default(Exception));

            _inactiveSleepTime = value;
        }
    }

    public bool IsActive { get; set; } = true;

    public void SuppressDrawForOneFrame()
    {
        _suppressDraw = true;
    }

    internal void Tick()
    {
    RetryTick:

        if (!IsActive && InactiveSleepTime.TotalMilliseconds >= 1.0)
        {
            Thread.Sleep((int)InactiveSleepTime.TotalMilliseconds);
        }

        // Advance the accumulated elapsed time.
        if (_gameTimer == null)
        {
            _gameTimer = new Stopwatch();
            _gameTimer.Start();
        }

        var currentTicks = _gameTimer.Elapsed.Ticks;
        _accumElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
        _previousTicks = currentTicks;

        if (IsFixedTimeStep && _accumElapsedTime < _targetElapsedTime)
        {
            // Sleep for as long as possible without overshooting the update time
            var sleepTime = (_targetElapsedTime - _accumElapsedTime).TotalMilliseconds;

            // We only have a precision timer on Windows, so other platforms may still overshoot

#if WINDOWS

            TimingUtils.SleepForNoMoreThan(sleepTime);
#else

            if (sleepTime >= 2.0)
            {
                Thread.Sleep(1);
            }

#endif
            goto RetryTick;
        }

        // Do not allow any update to take longer than our maximum.
        if (_accumElapsedTime > _maxElapsedTime)
        {
            _accumElapsedTime = _maxElapsedTime;
        }

        if (IsFixedTimeStep)
        {
            _gameTime.ElapsedGameTime = _targetElapsedTime;
            var stepCount = 0;

            // Perform as many full fixed length time steps as we can.
            while (_accumElapsedTime >= _targetElapsedTime && Running)
            {
                _gameTime.TotalGameTime += _targetElapsedTime;
                _accumElapsedTime -= _targetElapsedTime;
                ++stepCount;
                RunningScene?.InternalUpdate(_gameTime);
            }

            //Every update after the first accumulates lag
            _updateFrameLag += Math.Max(0, stepCount - 1);

            //If we think we are running slowly, wait until the lag clears before resetting it
            if (_gameTime.IsRunningSlowly)
            {
                if (_updateFrameLag == 0)
                {
                    _gameTime.IsRunningSlowly = false;
                }
            }
            else if (_updateFrameLag >= 5)
            {
                //If we lag more than 5 frames, start thinking we are running slowly
                _gameTime.IsRunningSlowly = true;
            }

            //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
            if (stepCount == 1 && _updateFrameLag > 0)
            {
                _updateFrameLag--;
            }

            // Draw needs to know the total elapsed time
            // that occured for the fixed length updates.
            _gameTime.ElapsedGameTime = TimeSpan.FromTicks(_targetElapsedTime.Ticks * stepCount);

        }
        else
        {
            // Perform a single variable length update.
            _gameTime.ElapsedGameTime = _accumElapsedTime;
            _gameTime.TotalGameTime += _accumElapsedTime;
            _accumElapsedTime = TimeSpan.Zero;

            RunningScene?.InternalUpdate(_gameTime);
        }

        if (_suppressDraw)
        {
            _suppressDraw = false;
        }
        else
        {
            RunningScene?.InternalDraw(_gameTime);
        }
    }
    private const int DefaultFrameRate = 60;

    private bool _suppressDraw;

    private int _targetFrameRate = DefaultFrameRate;

    private TimeSpan _targetElapsedTime = TimeSpan.FromTicks((long)(BooPlatform.GetPerformanceFrequency() / DefaultFrameRate));
    private TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);

    private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);

    private TimeSpan _accumElapsedTime;

    private readonly GameTime _gameTime = new();
    private Stopwatch? _gameTimer;
    private long _previousTicks;
    private int _updateFrameLag;
}
