using BepuPhysics;
using DProject.Game.Component;
using DProject.Game.Component.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class PhysicsSyncSystem : EntityUpdateSystem
    {
        private ComponentMapper<ActivePhysicsComponent> _physicsMapper;
        private ComponentMapper<TransformComponent> _transformMapper;

        private Simulation _simulation;
        
        public PhysicsSyncSystem(Simulation simulation) : base(Aspect.All(typeof(TransformComponent), typeof(ActivePhysicsComponent)))
        {
            _simulation = simulation;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _physicsMapper = mapperService.GetMapper<ActivePhysicsComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var physicsComponent = _physicsMapper.Get(entity);
                var transformComponent = _transformMapper.Get(entity);

                if (physicsComponent.Handle != null)
                {
                    if (physicsComponent.PreviousPosition != transformComponent.Position)
                    {
                        _simulation.Bodies.GetBodyReference((int) physicsComponent.Handle)
                            .Pose.Position = new global::System.Numerics.Vector3(
                            transformComponent.Position.X,
                            transformComponent.Position.Y,
                            transformComponent.Position.Z);
                    }

                    if (physicsComponent.PreviousRotation != transformComponent.Rotation)
                    {
                        _simulation.Bodies.GetBodyReference((int) physicsComponent.Handle)
                            .Pose.Orientation = new BepuUtilities.Quaternion(
                            transformComponent.Rotation.X,
                            transformComponent.Rotation.Y,
                            transformComponent.Rotation.Z,
                            transformComponent.Rotation.W);
                    }

                    var pose = _simulation.Bodies.GetBodyReference((int) physicsComponent.Handle).Pose;

                    transformComponent.Position = new Vector3(pose.Position.X, pose.Position.Y, pose.Position.Z);
                    transformComponent.Rotation = new Quaternion(pose.Orientation.X, pose.Orientation.Y, pose.Orientation.Z, pose.Orientation.W);
                    
                    physicsComponent.PreviousPosition = transformComponent.Position;
                    physicsComponent.PreviousRotation = transformComponent.Rotation;
                }
            }
        }
    }
}
