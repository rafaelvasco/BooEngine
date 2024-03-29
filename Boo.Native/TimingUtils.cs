using System.Runtime.InteropServices;

namespace Boo.Native;

internal static class TimingUtils
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtQueryTimerResolution(out uint MinimumResolution, out uint MaximumResolution, out uint CurrentResolution);

    private static readonly double LowestSleepThreshold;

    static TimingUtils()
    {
        NtQueryTimerResolution(out _, out var max, out _);
        LowestSleepThreshold = 1.0 + (max / 10000.0);
    }

    /// <summary>
    /// Returns the current timer resolution in milliseconds
    /// </summary>
    public static double GetCurrentResolution()
    {
        NtQueryTimerResolution(out _, out _, out var current);
        return current / 10000.0;
    }

    /// <summary>
    /// Sleeps as long as possible without exceeding the specified period
    /// </summary>
    public static void SleepForNoMoreThan(double milliseconds)
    {
        // Assumption is that Thread.Sleep(t) will sleep for at least (t), and at most (t + timerResolution)
        if (milliseconds < LowestSleepThreshold)
            return;
        var sleepTime = (int)(milliseconds - GetCurrentResolution());
        if (sleepTime > 0)
            Thread.Sleep(sleepTime);
    }
}
