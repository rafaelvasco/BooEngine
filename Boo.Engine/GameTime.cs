namespace Boo.Engine;

/// <summary>
/// Holds the time state of a <see cref="BooEngine"/>.
/// </summary>
public class GameTime
{
    /// <summary>
    /// Time since the start of the Game/>.
    /// </summary>
    public TimeSpan TotalGameTime { get; set; }

    /// <summary>
    /// Time since the last Update.
    /// </summary>
    public TimeSpan ElapsedGameTime { get; set; }


    public float FrameDeltaSec => (float)ElapsedGameTime.TotalSeconds;

    /// <summary>
    /// Indicates whether the <see cref="BooGameLoop"/> is running slowly.
    ///
    /// This flag is set to <c>true</c> when <see cref="BooGameLoop.IsFixedTimeStep"/> is set to <c>true</c>
    /// and a tick of the game loop takes longer than <see cref="BooGameLoop._targetElapsedTime"/> for
    /// a few frames in a row.
    /// </summary>
    public bool IsRunningSlowly { get; set; }

    /// <summary>
    /// Create a <see cref="GameTime"/> instance with a <see cref="TotalGameTime"/> and
    /// <see cref="ElapsedGameTime"/> of <code>0</code>.
    /// </summary>
    public GameTime()
    {
        TotalGameTime = TimeSpan.Zero;
        ElapsedGameTime = TimeSpan.Zero;
        IsRunningSlowly = false;
    }

    /// <summary>
    /// Create a <see cref="GameTime"/> with the specified <see cref="TotalGameTime"/>
    /// and <see cref="ElapsedGameTime"/>.
    /// </summary>
    /// <param name="totalGameTime">The total game time elapsed since the start of the Game.</param>
    /// <param name="elapsedGameTime">The time elapsed since the last Update.</param>
    public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
    {
        TotalGameTime = totalGameTime;
        ElapsedGameTime = elapsedGameTime;
        IsRunningSlowly = false;
    }

    /// <summary>
    /// Create a <see cref="GameTime"/> with the specified <see cref="TotalGameTime"/>
    /// and <see cref="ElapsedGameTime"/>.
    /// </summary>
    /// <param name="totalRealTime">The total game time elapsed since the start of the Game.</param>
    /// <param name="elapsedRealTime">The time elapsed since the last Update.</param>
    /// <param name="isRunningSlowly">A value indicating if the Game is running slowly.</param>
    public GameTime(TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
    {
        TotalGameTime = totalRealTime;
        ElapsedGameTime = elapsedRealTime;
        IsRunningSlowly = isRunningSlowly;
    }
}