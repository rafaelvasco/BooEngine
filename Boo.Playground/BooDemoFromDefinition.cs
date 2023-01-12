using Boo.Engine.Toolkit;

namespace Boo.Playground;

public class BooDemoFromDefinition : BooScene
{
    public override void Load()
    {
        LoadFromDefinition("dataBasedScene");
    }
}