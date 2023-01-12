using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public enum Ease
{
    Linear,
    InSine,
    OutSine,
    InOutSine,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InExpo,
    OutExpo,
    InOutExpo,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce
}

public static class Interpolator
{
    public static float Calculate(Ease ease, float start, float end, float currentTime, float duration)
    {
        float t = currentTime;
        float b = start;
        float d = duration;
        float c = end - start;

        switch (ease)
        {
            case Ease.Linear:
                return c * t / d + b;
            case Ease.InSine:
                return -c * Calc.Cos(t / d * (Calc.PI / 2)) + c + b;
            case Ease.OutSine:
                return c * Calc.Sin(t / d * (Calc.PI / 2)) + b;
            case Ease.InOutSine:
                return -c / 2 * (Calc.Cos(Calc.PI * t / d) - 1) + b;
            case Ease.InQuad:
                t /= d;
                return c * t * t + b;
            case Ease.OutQuad:
                t /= d;
                return -c * t * (t - 2) + b;
            case Ease.InOutQuad:
                t /= d / 2;
                if (t < 1) return c / 2 * t * t + b;
                t--;
                return -c / 2 * (t * (t - 2) - 1) + b;
            case Ease.InCubic:
                t /= d;
                return c * t * t * t + b;
            case Ease.OutCubic:
                t /= d;
                t--;
                return c * (t * t * t + 1) + b;
            case Ease.InOutCubic:
                t /= d / 2;
                if (t < 1) return c / 2 * t * t * t + b;
                t -= 2;
                return c / 2 * (t * t * t + 2) + b;
            case Ease.InQuart:
                t /= d;
                return c * t * t * t * t + b;
            case Ease.OutQuart:
                t /= d;
                t--;
                return -c * (t * t * t * t - 1) + b;
            case Ease.InOutQuart:
                t /= d / 2;
                if (t < 1) return c / 2 * t * t * t * t + b;
                t -= 2;
                return -c / 2 * (t * t * t * t - 2) + b;
            case Ease.InQuint:
                t /= d;
                return c * t * t * t * t * t + b;
            case Ease.OutQuint:
                t /= d;
                t--;
                return c * (t * t * t * t * t + 1) + b;
            case Ease.InOutQuint:
                t /= d / 2;
                if (t < 1) return c / 2 * t * t * t * t * t + b;
                t -= 2;
                return c / 2 * (t * t * t * t * t + 2) + b;
            case Ease.InExpo:
                return c * Calc.Pow(2, 10 * (t / d - 1)) + b;
            case Ease.OutExpo:
                return c * (-Calc.Pow(2, -10 * t / d) + 1) + b;
            case Ease.InOutExpo:
                if (t < 1) return c / 2 * Calc.Pow(2, 10 * (t - 1)) + b;
                t--;
                return c / 2 * (-Calc.Pow(2, -10 * t) + 2) + b;
            case Ease.InCirc:
                t /= d;
                return -c * (Calc.Sqrt(1 - t * t) - 1) + b;
            case Ease.OutCirc:
                t /= d;
                t--;
                return c * Calc.Sqrt(1 - t * t) + b;
            case Ease.InOutCirc:
                t /= d / 2;
                if (t < 1) return -c / 2 * (Calc.Sqrt(1 - t * t) - 1) + b;
                t -= 2;
                return c / 2 * (Calc.Sqrt(1 - t * t) + 1) + b;
            case Ease.InElastic:
                if (t == 0) return b;
                if (Calc.ApproximatelyEqual(t /= d, 1.0f)) return b + c;
                float p = d * .3f;
                float a = c;
                float s = p / 4;
                return -(a * Calc.Pow(2, 10 * (t -= 1)) *
                         Calc.Sin((t * d - s) * (2 * Calc.PI) / p)) + b;
            case Ease.OutElastic:
                if (t == 0) return b;
                if (Calc.ApproximatelyEqual(t /= d, 1.0f)) return b + c;
                p = d * .3f;
                a = c;
                s = p / 4;
                return a * Calc.Pow(2, -10 * t) *
                    Calc.Sin((t * d - s) * (2 * Calc.PI) / p) + c + b;
            case Ease.InOutElastic:
                if (t == 0) return b;
                if (Calc.ApproximatelyEqual(t /= d / 2f, 2.0f))
                {
                    return b + c;
                }

                p = d * (.3f * 1.5f);
                a = c;
                s = p / 4;
                if (t < 1)
                    return -.5f * (a * Calc.Pow(2, 10 * (t -= 1)) *
                                   Calc.Sin((t * d - s) * (2 * Calc.PI) / p)) + b;
                return a * Calc.Pow(2, -10 * (t -= 1)) *
                    Calc.Sin((t * d - s) * (2 * Calc.PI) / p) * .5f + c + b;
            case Ease.InBack:
                s = 1.70158f;
                return c * (t /= d) * t * ((s + 1) * t - s) + b;
            case Ease.OutBack:
                s = 1.70158f;
                return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
            case Ease.InOutBack:
                s = 1.70158f;
                if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= 1.525f) + 1) * t - s)) + b;
                return c / 2 * ((t -= 2) * t * (((s *= 1.525f) + 1) * t + s) + 2) + b;
            case Ease.InBounce:
                return EaseInBounce(t, b, c, d);
            case Ease.OutBounce:
                return EaseOutBounce(t, b, c, d);
            case Ease.InOutBounce:
                if (t < d / 2) return EaseInBounce(t * 2, 0, c, d) * .5f + b;
                return EaseOutBounce(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
            default:
                throw new ArgumentOutOfRangeException(nameof(ease), ease, null);
        }
    }

    private static float EaseInBounce(float t, float b, float c, float d)
    {
        return c - EaseOutBounce(d - t, 0, c, d) + b;
    }

    private static float EaseOutBounce(float t, float b, float c, float d)
    {
        if ((t /= d) < 1f / 2.75f)
        {
            return c * (7.5625f * t * t) + b;
        }

        return t switch
        {
            < 2f / 2.75f => c * (7.5625f * (t -= 1.5f / 2.75f) * t + .75f) + b,
            < 2.5f / 2.75f => c * (7.5625f * (t -= 2.25f / 2.75f) * t + .9375f) + b,
            _ => c * (7.5625f * (t -= 2.625f / 2.75f) * t + .984375f) + b
        };
    }
}