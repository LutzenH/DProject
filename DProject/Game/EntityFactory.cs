using DProject.Game.Component;
using DProject.Game.Component.Ports;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace DProject.Game
{
    public class EntityFactory
    {
        public const float WaterPlaneHeight = 10f;
        
        private readonly World _world;
        
        public EntityFactory(World world)
        {
            _world = world;
        }

        public Entity CreateGameTime()
        {
            var entity = _world.CreateEntity();
            
            entity.Attach(new GameTimeComponent());

            return entity;
        }
        
        public Entity CreateFlyCamera()
        {
            var entity = _world.CreateEntity();
            
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
            var entity = _world.CreateEntity();
            
            entity.Attach(new TransformComponent());
            entity.Attach(new ModelComponent(3));

            return entity;
        }

        public Entity CreateWaterPlane()
        {
            var entity = _world.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = new Vector3(0, WaterPlaneHeight, 0)
            });
            entity.Attach(new WaterPlaneComponent());

            return entity;
        }
    }
}
