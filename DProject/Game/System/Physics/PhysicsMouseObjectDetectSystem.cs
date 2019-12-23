using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using DProject.Game.Component;
using DProject.Game.Component.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class PhysicsMouseObjectDetectSystem : EntityUpdateSystem
    {
        private readonly Simulation _simulation;

        private ComponentMapper<ActivePhysicsComponent> _activePhysicsMapper;
        private ComponentMapper<StaticPhysicsComponent> _staticPhysicsMapper;
        
        public PhysicsMouseObjectDetectSystem(Simulation simulation) : base(Aspect.One(typeof(ActivePhysicsComponent), typeof(StaticPhysicsComponent)))
        {
            _simulation = simulation;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _activePhysicsMapper = mapperService.GetMapper<ActivePhysicsComponent>();
            _staticPhysicsMapper = mapperService.GetMapper<StaticPhysicsComponent>();
        }
        
        public override void Update(GameTime gameTime)
        {
#if EDITOR
            if (InputManager.Instance.IsInputPressed(Input.PickupPhysicsBody))
                DebugUIRenderSystem.SelectedEntity = GetEntityAtMouse();
#endif
        }

        private int? GetEntityAtMouse()
        {
            var collidableReference = GetCollidableAtMouse(_simulation, CameraSystem.ActiveLens);

            if (collidableReference != null)
            {
                foreach (var entity in ActiveEntities)
                {
                    if (_activePhysicsMapper.Has(entity) && collidableReference.Value.Mobility == CollidableMobility.Dynamic)
                    {
                        var physicsComponent = _activePhysicsMapper.Get(entity);

                        if (physicsComponent.Handle == collidableReference.Value.Handle)
                            return entity;
                    }
                    else if (_staticPhysicsMapper.Has(entity) && collidableReference.Value.Mobility == CollidableMobility.Static)
                    {
                        var physicsComponent = _staticPhysicsMapper.Get(entity);

                        if (physicsComponent.Handle == collidableReference.Value.Handle)
                            return entity;
                    }
                }
            }

            return null;
        }

        private static CollidableReference? GetCollidableAtMouse(Simulation simulation, LensComponent lens)
        {
            var lensDirection = new global::System.Numerics.Vector3(lens.Direction.X, lens.Direction.Y, lens.Direction.Z);
            var lensPosition = new global::System.Numerics.Vector3(lens.Position.X, lens.Position.Y, lens.Position.Z);
            
            var rayDirection = lensDirection;
            var hitHandler = default(RayHitHandler);
            hitHandler.T = float.MaxValue;
            simulation.RayCast(lensPosition, rayDirection, float.MaxValue, ref hitHandler);
            
            if (hitHandler.T < float.MaxValue)
                return hitHandler.HitCollidable;

            return null;
        }

        private struct RayHitHandler : IRayHitHandler
        {
            public float T;
            public CollidableReference HitCollidable;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowTest(CollidableReference collidable)
            {
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowTest(CollidableReference collidable, int childIndex)
            {
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnRayHit(in RayData ray, ref float maximumT, float t, in global::System.Numerics.Vector3 normal, CollidableReference collidable, int childIndex)
            {
                //We are only interested in the earliest hit. This callback is executing within the traversal, so modifying maximumT informs the traversal
                //that it can skip any AABBs which are more distant than the new maximumT.
                if (t < maximumT)
                    maximumT = t;
                if (t < T)
                {
                    //Cache the earliest impact.
                    T = t;
                    HitCollidable = collidable;
                }
            }
        }
    }
} 
