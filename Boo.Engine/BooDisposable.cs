using Boo.Common;

namespace Boo.Engine;

public abstract class BooDisposable : IDisposable
{
    ~BooDisposable()
    {   
        Dispose();
        throw new BooException("Not properly disposed");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Free();
    }

    protected abstract void Free();
}