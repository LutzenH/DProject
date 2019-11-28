using System;
using BepuPhysics.Collidables;
using DProject.Game.Component;
using DProject.Game.Component.Lighting;
using DProject.Game.Component.Physics;
using DProject.Type.Rendering.Primitives;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace DProject.Game
{
    public class EntityFactory
    {
        public World World { get; set; }

        public Entity CreateFlyCamera(Vector3 position)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new LensComponent()
            {
                Position = position,
                ReflectionPlaneHeight = position.Y
            });
            
            entity.Attach(new FlyCameraComponent()
            {
                Speed = 60f
            });

            return entity;
        }

        #region Primitives

        public Entity CreatePrimitive(Vector3 position, Vector3 scale, Quaternion rotation, PrimitiveType type)
        {
            var entity = World.CreateEntity();

            entity.Attach(new TransformComponent()
            {
                Position = position,
                Scale = scale,
                Rotation = rotation,
            });
            
            entity.Attach(new PrimitiveComponent()
            {
                Type = type
            });

            return entity;
        }
        
        public Entity CreatePrimitive(Vector3 fromPosition, Vector3 toPosition, PrimitiveType type)
        {
            var entity = World.CreateEntity();

            var position = (fromPosition + toPosition) / 2;
            var scale = new Vector3(Math.Abs(fromPosition.X - toPosition.X),
                                    Math.Abs(fromPosition.Y - toPosition.Y),
                                    Math.Abs(fromPosition.Z - toPosition.Z));
            
            entity.Attach(new TransformComponent()
            {
                Position = position,
                Scale = scale,
                Rotation = Quaternion.Identity,
            });
            
            entity.Attach(new PrimitiveComponent()
            {
                Type = type
            });

            return entity;
        }

        public Entity CreatePhysicsStaticPrimitive(Vector3 fromPosition, Vector3 toPosition, PrimitiveType type)
        {
            var entity = World.CreateEntity();
            
            var position = (fromPosition + toPosition) / 2;
            var scale = new Vector3(Math.Abs(fromPosition.X - toPosition.X),
                Math.Abs(fromPosition.Y - toPosition.Y),
                Math.Abs(fromPosition.Z - toPosition.Z));
            
            entity.Attach(new TransformComponent()
            {
                Position = position,
                Scale = scale,
                Rotation = Quaternion.Identity,
            });
            
            entity.Attach(new PrimitiveComponent()
            {
                Type = type
            });

            IConvexShape shape;
            switch (type)
            {
                case PrimitiveType.Sphere:
                    shape = new Sphere(scale.Length()/2);
                    break;
                case PrimitiveType.Cube:
                default:
                    shape = new Box(scale.X, scale.Y, scale.Z);
                    break;
            }
            
            entity.Attach(new PhysicsComponent()
            {
                StartPosition = new System.Numerics.Vector3(position.X, position.Y, position.Z),
                Shape = shape,
                SpeculativeMargin = 0.1f
            });

            return entity;
        }
        
        public Entity CreatePhysicsDynamicPrimitive(Vector3 position, Vector3 scale, Quaternion rotation, PrimitiveType type)
        {
            var entity = World.CreateEntity();

            entity.Attach(new TransformComponent()
            {
                Position = position,
                Scale = scale,
                Rotation = rotation,
            });
            
            entity.Attach(new PrimitiveComponent()
            {
                Type = type
            });
            
            IConvexShape shape;
            switch (type)
            {
                case PrimitiveType.Sphere:
                    shape = new Sphere(scale.Length()/2);
                    break;
                case PrimitiveType.Cube:
                default:
                    shape = new Box(scale.X, scale.Y, scale.Z);
                    break;
            }
            
            entity.Attach(new PhysicsBodyComponent()
            {
                SleepTreshold = 0.01f,
                Mass = 1f,
                StartPosition = new System.Numerics.Vector3(position.X, position.Y, position.Z),
                Shape = shape,
                SpeculativeMargin = 0.1f
            });

            return entity;
        }
        
        #endregion

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
        
        public Entity CreateProp(Vector3 position, string path)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = position
            });
            entity.Attach(new ModelComponent(path));

            return entity;
        }

        public Entity CreateWaterPlane(Vector3 position, Vector2 size)
        {
            var entity = World.CreateEntity();
            
            entity.Attach(new TransformComponent()
            {
                Position = new Vector3(position.X, position.Y, position.Z)
            });
            entity.Attach(new WaterPlaneComponent()
            {
                Size = size
            });

            return entity;
        }

        #region Lights
        
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
        
        #endregion
    }
}
