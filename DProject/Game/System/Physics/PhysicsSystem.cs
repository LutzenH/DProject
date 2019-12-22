using System;
using System.Runtime.CompilerServices;
using System.Timers;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using DProject.Game.Component.Physics;
using DProject.Type;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public static class SelfContainedSimulation
    {
        public unsafe struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
        {
            public void Initialize(Simulation simulation) { }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
            {
                return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
            {
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : struct, IContactManifold<TManifold>
            {
                pairMaterial.FrictionCoefficient = 1f;
                pairMaterial.MaximumRecoveryVelocity = 2f;
                pairMaterial.SpringSettings = new SpringSettings(30, 1);

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
            {
                return true;
            }

            public void Dispose() { }
        }
        
        public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
        {
            public global::System.Numerics.Vector3 Gravity;
            global::System.Numerics.Vector3 gravityDt;
            
            public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

            public PoseIntegratorCallbacks(global::System.Numerics.Vector3 gravity) : this()
            {
                Gravity = gravity;
            }
            
            public void PrepareForIntegration(float dt)
            {
                gravityDt = Gravity * dt;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
            {
                if (localInertia.InverseMass > 0)
                    velocity.Linear = velocity.Linear + gravityDt;
            }
        }
    }

    public class PhysicsSystem : EntityUpdateSystem, IDisposable
    {
        private ComponentMapper<PhysicsComponent> _physicsMapper;
        private ComponentMapper<PhysicsBodyComponent> _physicsBodyMapper;
        
        private ComponentMapper<ActivePhysicsComponent> _activePhysicsMapper;
        private ComponentMapper<StaticPhysicsComponent> _staticPhysicsMapper;

        private readonly Simulation _simulation;
        private readonly BufferPool _bufferPool;

        private readonly SimpleThreadDispatcher _threadDispatcher;
        
        private readonly Timer _simulationStepTimer;
        private const int SimulationTimeStepPerSecond = 60;

        public bool EnableSimulation {
            get => _simulationStepTimer.Enabled;
            set => _simulationStepTimer.Enabled = value;
        }

        public PhysicsSystem() : base(Aspect.One(typeof(PhysicsComponent), typeof(PhysicsBodyComponent)))
        {
            _bufferPool = new BufferPool();
            _simulation = Simulation.Create(_bufferPool, new SelfContainedSimulation.NarrowPhaseCallbacks(), new SelfContainedSimulation.PoseIntegratorCallbacks(new global::System.Numerics.Vector3(0, -10, 0)));

            _threadDispatcher = new SimpleThreadDispatcher(Environment.ProcessorCount);
            
            _simulationStepTimer = new Timer(1000f / SimulationTimeStepPerSecond);
            _simulationStepTimer.Elapsed += OnSimulationTimeStep;
            
#if EDITOR
            _simulationStepTimer.Enabled = false;
#else
            _simulationStepTimer.Enabled = true;
#endif
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
            _physicsBodyMapper = mapperService.GetMapper<PhysicsBodyComponent>();

            _activePhysicsMapper = mapperService.GetMapper<ActivePhysicsComponent>();
            _staticPhysicsMapper = mapperService.GetMapper<StaticPhysicsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                if (_physicsMapper.Has(entity)) {
                    var physicsComponent = _physicsMapper.Get(entity);

                    int? componentHandle = null;
                    if (physicsComponent.Shape is Box)
                        componentHandle = _simulation.Statics.Add(new StaticDescription(physicsComponent.StartPosition, new CollidableDescription(_simulation.Shapes.Add((Box) physicsComponent.Shape), physicsComponent.SpeculativeMargin)));
                    else if (physicsComponent.Shape is Sphere)
                        componentHandle = _simulation.Statics.Add(new StaticDescription(physicsComponent.StartPosition, new CollidableDescription(_simulation.Shapes.Add((Sphere) physicsComponent.Shape), physicsComponent.SpeculativeMargin)));
                    
                    _physicsMapper.Delete(entity);
                    _staticPhysicsMapper.Put(entity, new StaticPhysicsComponent() { Handle = componentHandle });   
                }
                else if (_physicsBodyMapper.Has(entity))
                {
                    var physicsBodyComponent = _physicsBodyMapper.Get(entity);
                    
                    physicsBodyComponent.Shape.ComputeInertia(physicsBodyComponent.Mass, out var bodyInertia);

                    int? componentHandle = null;
                    if (physicsBodyComponent.Shape is Box)
                        componentHandle = _simulation.Bodies.Add(BodyDescription.CreateDynamic(physicsBodyComponent.StartPosition, bodyInertia, new CollidableDescription(_simulation.Shapes.Add((Box) physicsBodyComponent.Shape), physicsBodyComponent.SpeculativeMargin), new BodyActivityDescription(physicsBodyComponent.SleepTreshold)));
                    else if (physicsBodyComponent.Shape is Sphere)
                        componentHandle = _simulation.Bodies.Add(BodyDescription.CreateDynamic(physicsBodyComponent.StartPosition, bodyInertia, new CollidableDescription(_simulation.Shapes.Add((Sphere) physicsBodyComponent.Shape), physicsBodyComponent.SpeculativeMargin), new BodyActivityDescription(physicsBodyComponent.SleepTreshold)));
                    
                    _physicsBodyMapper.Delete(entity);
                    _activePhysicsMapper.Put(entity, new ActivePhysicsComponent() { Handle = componentHandle });   
                }
            }
        }

        private void OnSimulationTimeStep(object source, ElapsedEventArgs e)
        {
            _simulation.Timestep(0.01f, _threadDispatcher);
        }

        public Simulation GetSimulation()
        {
            return _simulation;
        }
        
        public new void Dispose()
        {
            _simulation.Dispose();
            _threadDispatcher.Dispose();
            _bufferPool.Clear();
            
            base.Dispose();
        }
    }
}
