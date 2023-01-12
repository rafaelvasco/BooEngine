using System.Numerics;
using System.Text.Json.Nodes;
using Boo.Common.Math;

namespace Boo.Engine.Toolkit;

public enum ComponentType
{
    DragAndDrop,
    DirectionalMovement,
    TileMovement,
    WaveMovement,
    TimelineAnimation
}

public class ComponentDefinition
{
    public ComponentType Type { get; init; }
    public JsonObject? Data { get; init; }
}

public class BaseNodeDefinition
{
    public string? Name { get; init; }

    public bool Visible { get; init; } = true;
    
    public float X { get; init; }
    
    public float Y { get; init; }
    
    public ComponentDefinition[]? Components { get; init; }
}

public struct BooTileDefinition
{
    public int TileIndex;
    public int TileX;
    public int TileY;
}

/// <summary>
/// Tilemap Definition
/// </summary>
public class BooTileMapDefinition : BaseNodeDefinition
{
    /// <summary>
    /// Which tileset to use
    /// </summary>
    public string? TileSet { get; init; }
    
    /// <summary>
    /// List of tiles in format (int TileIndex, int TileX, int TileY)
    /// </summary>
    public BooTileDefinition[]? Tiles { get; init; }
    
    /// <summary>
    /// Map Width in Tile Count
    /// </summary>
    public int MapWidth { get; init; }
    
    /// <summary>
    /// Map Height in Tile Count
    /// </summary>
    public int MapHeight { get; init; }
}

public class BooSpriteDefinition : BaseNodeDefinition
{
    /// <summary>
    /// Texture2D asset id
    /// </summary>
    public string? Texture { get; init; }

    /// <summary>
    /// Color Tint
    /// </summary>
    public Color ColorTint { get; init; } = Color.White;
    
    /// <summary>
    /// Texture region or empty if none
    /// </summary>
    public Rect SourceRect { get; init; }
    
    /// <summary>
    /// Override size or emtpy if none
    /// </summary>
    public Size Size { get; init; }

    /// <summary>
    /// Origin 0.0 to 1.0. Defaults to 0.5,0.5
    /// </summary>
    public Vector2 Origin { get; init; } = new(0.5f, 0.5f);
}

public class BooSceneDefinition
{
    public List<BooTileMapDefinition>? Tilemaps { get; init; }
    
    public List<BooSpriteDefinition>? Sprites { get; init; }
}