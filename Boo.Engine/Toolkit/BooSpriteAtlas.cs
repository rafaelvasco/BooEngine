using Boo.Common;
using Boo.Common.Math;
using Boo.Engine.Content;
using Boo.Engine.Graphics;

namespace Boo.Engine.Toolkit;

public class BooSpriteAtlas : BooAsset
{
    public Texture2D Texture { get; }
    
    public Quad this[string name]
    {
        get
        {
            if (_quadMap.TryGetValue(name, out int index))
            {
                return _quads[index];
            }

            throw new BooException($"Can't find sprite with name {name} on this BooSpriteAtlas");
        }
    } 

    internal BooSpriteAtlas(string id, Texture2D texture) : base(id)
    {
        Texture = texture;
        _quads = new List<Quad>();
        _quadMap = new Dictionary<string, int>();
    }

    public void AddSprite(string name, Rect region)
    {
        var quad = new Quad(Texture, region);
        
        _quads.Add(quad);
        
        _quadMap.Add(name, _quads.Count-1);
    }

    private readonly List<Quad> _quads;

    private Dictionary<string, int> _quadMap;
    protected override void Free()
    {
    }
}