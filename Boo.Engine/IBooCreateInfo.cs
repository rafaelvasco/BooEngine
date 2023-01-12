using Boo.Common;

namespace Boo.Engine;

public interface IBooCreateInfo
{
    public bool IsValid();
}

public static class BooCreateInfoHelper
{
    /// <summary>
    /// Throws <see cref="BooException"/> if the fields are not properly configured to be passed
    /// in as constructor parameters.
    /// </summary>
    /// <param name="self">The <see cref="IBooCreateInfo"/> instance.</param>
    /// <exception cref="BooException">Thrown if <see cref="IBooCreateInfo"/> is
    /// not valid.</exception>
    public static void ThrowIfInvalid(this IBooCreateInfo self)
    {
        if (!self.IsValid())
            throw new BooException(self.GetType().ToString());
    }
}