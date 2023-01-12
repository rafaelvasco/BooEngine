namespace Boo.Engine.Content;

public abstract class BooAsset : BooDisposable
{
    public string Id { get; }

    protected BooAsset(string id)
    {
        Id = id;
    }
}
