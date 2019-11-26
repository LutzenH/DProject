using System;
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

                if (physicsComponent.ComponentHandle != null)
                {
                    if (physicsComponent.BodyType == BodyType.Body)
                    {
                        var pose = _simulation.Bodies.GetBodyReference((int) physicsComponent.ComponentHandle).Pose;

                        transformComponent.Position = new Vector3(pose.Position.X, pose.Position.Y, pose.Position.Z);
                        transformComponent.Rotation = new Quaternion(pose.Orientation.X, pose.Orientation.Y, pose.Orientation.Z, pose.Orientation.W);
                    }
                }
            }
        }
    }
}
