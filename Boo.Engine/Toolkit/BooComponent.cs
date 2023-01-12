using System.Dynamic;
using System.Text.Json.Nodes;

namespace Boo.Engine.Toolkit;

public abstract class BooComponent
{
    public bool Enabled { get; set; } = true;
    
    public BooNode? Parent { get; internal set; }
    
    public abstract void Update(GameTime time);
    
    public virtual void OnAttached(BooNode parent) {}
    
    public virtual void SetParametersFromDefinitionData(JsonObject data) {}
}