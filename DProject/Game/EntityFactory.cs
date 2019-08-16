using DProject.Game.Component;
using DProject.Game.Component.Ports;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace DProject.Game
{
    public class EntityFactory
    {
        public const float WaterPlaneHeight = 5f;

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

        public Entity CreateProp()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent());
            entity.Attach(new ModelComponent(3));

            return entity;
        }

        public Entity CreateWaterPlane()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = new Vector3(0, WaterPlaneHeight, 0)
            });
            entity.Attach(new WaterPlaneComponent());

            return entity;
        }

        public Entity CreateHeightmap(Vector3 position, Vertex[,] heightmap)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = position
            });
            entity.Attach(new HeightmapComponent()
            {
                Heightmap = heightmap
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
    }
}
