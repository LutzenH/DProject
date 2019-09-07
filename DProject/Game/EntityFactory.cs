using DProject.Game.Component;
using DProject.Game.Component.Lighting;
using DProject.Game.Component.Ports;
using DProject.Game.Component.Terrain;
using DProject.Game.Component.Terrain.ClipMap;
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
        
        public Entity CreateFlyCamera(Vector3 position)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new LensComponent()
            {
                Position = position,
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

        public Entity CreateTerrainEntity()
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new ClipMapTerrainComponent());

            return entity;
        }

        public Entity CreateClipMap(ClipMapType type)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new ClipMapComponent()
            {
                Type = type
            });

            return entity;
        }

        public Entity CreateDirectionalLight(Vector3 direction, Color color)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new DirectionalLightComponent()
            {
                Direction = direction,
                Color = color.ToVector3()
            });

            return entity;
        }
        
        public Entity CreatePointLight(Vector3 position, Color color, float radius, float intensity)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new PointLightComponent()
            {
                Position = position,
                Color = color.ToVector3(),
                Radius = radius,
                Intensity = MathHelper.Clamp(intensity, 0f, 1f)
            });

            return entity;
        }
    }
}
