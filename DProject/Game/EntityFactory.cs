using DProject.Game.Component;
using DProject.Game.Component.Ports;
using DProject.Game.Component.Terrain;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace DProject.Game
{
    public class EntityFactory
    {
        public const float WaterPlaneHeight = 96f;

        public World World { get; set; }
        
        public Entity CreateGameTime()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new GameTimeComponent());

            return entity;
        }
        
        public Entity CreateFlyCamera()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new LensComponent()
            {
                Position = new Vector3(0, 10, 0),
                ReflectionPlaneHeight = WaterPlaneHeight
            });
            
            entity.Attach(new FlyCameraComponent()
            {
                Speed = 60f
            });

            return entity;
        }

        public Entity CreateProp(Vector3 position, ushort id)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = position
            });
            entity.Attach(new ModelComponent(id));

            return entity;
        }

        public Entity CreateWaterPlane(Vector2 position, Vector2 size)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = new Vector3(position.X, WaterPlaneHeight, position.Y)
            });
            entity.Attach(new WaterPlaneComponent()
            {
                Size = size
            });

            return entity;
        }

        public Entity CreateHeightmap(Vector3 position, Vertex[,] heightmap, VertexBuffer recycledVertexBuffer = null)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = position
            });
            entity.Attach(new HeightmapComponent()
            {
                Heightmap = heightmap,
                RecycledVertexBuffer = recycledVertexBuffer
            });
            entity.Attach(new BoundingBoxComponent()
            {
                BoundingBox = new BoundingBox(
                    position,
                    new Vector3(
                        position.X + heightmap.GetLength(0) - 1,
                        float.MaxValue, 
                        position.Z + heightmap.GetLength(1) - 1))
            });

            return entity;
        }

        public Entity CreateTerrainEntity()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new ClipMapTerrainComponent());

            return entity;
        }
    }
}
