using System.Text.Json;
using System.Text.Json.Serialization;
using Boo.Engine.Content;

namespace Boo.Engine.Toolkit;

public abstract class BooScene
{
    private BooContainer _container;
    private BooCanvas _canvas;

    public BooScene()
    {
        _container = new BooContainer("mainContainer");
        _canvas = new BooCanvas();
    }

    public void Add(BooNode node)
    {
        _container.AddChild(node);
    }

    public abstract void Load();

    internal void InternalUpdate(GameTime time)
    {
        _container.InternalProcess(time);
        
        Update(time);
    }

    internal void InternalDraw(GameTime time)
    {
        _canvas.Begin();
        _container.Draw(_canvas);
        Draw(_canvas);
        _canvas.End();
    }

    public virtual void Update(GameTime time) {}

    public virtual void Draw(BooCanvas canvas) {}

    public void LoadFromDefinition(string definitionFileName)
    {
        void AddComponents(BooNode node, ComponentDefinition[] componentDefinitions)
        {
            foreach (var componentDef in componentDefinitions)
            {
                BooComponent component;
                switch (componentDef.Type)
                {
                    case ComponentType.DragAndDrop:
                        component = node.AddComponent<DragAndDrop>();
                        break;
                    case ComponentType.DirectionalMovement:
                        component = node.AddComponent<DirectionalMovement>();
                        break;
                    case ComponentType.TileMovement:
                        component = node.AddComponent<TileMovement>();
                        break;
                    case ComponentType.WaveMovement:
                        component = node.AddComponent<WaveMovement>();
                        break;
                    case ComponentType.TimelineAnimation:
                        component = node.AddComponent<TimelineAnimation>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (componentDef.Data != null)
                {
                    component.SetParametersFromDefinitionData(componentDef.Data);
                }
            }
        }

        void SetCommonData(BooNode node, BaseNodeDefinition baseDefinition)
        {
            node.X = baseDefinition.X;
            node.Y = baseDefinition.Y;
            node.Visible = baseDefinition.Visible;
            node.Name = baseDefinition.Name ?? $"Node_{node.UId}";
        }
        
        var sceneDefinition = BooContent.LoadDefinitionObject<BooSceneDefinition>(definitionFileName, new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        });

        if (sceneDefinition.Tilemaps != null)
        {
            foreach (var tileMapDefinition in sceneDefinition.Tilemaps)
            {
                var tilemap = BooTileMap.LoadFromDefinition(tileMapDefinition);

                SetCommonData(tilemap, tileMapDefinition);

                if (tileMapDefinition.Components is { Length: > 0 })
                {
                    AddComponents(tilemap, tileMapDefinition.Components);
                }
                
                Add(tilemap);
            }
        }

        if (sceneDefinition.Sprites != null)
        {
            foreach (var spriteDefinition in sceneDefinition.Sprites)
            {
                var sprite = BooSprite.LoadFromDefinition(spriteDefinition);
                
                SetCommonData(sprite, spriteDefinition);

                if (spriteDefinition.Components is { Length: > 0 })
                {
                    AddComponents(sprite, spriteDefinition.Components);
                }
                
                Add(sprite);
            }
        }
    }
}