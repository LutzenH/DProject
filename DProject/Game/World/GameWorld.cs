using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Lighting;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        public readonly EntityFactory EntityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(Game1 game, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            EntityFactory = new EntityFactory();

            //Camera
            AddSystem(new CameraSystem());

            //Models
            AddSystem(new ModelLoaderSystem(graphicsDevice, contentManager));
            AddSystem(new ModelRenderSystem(graphicsDevice));

            //Physics
            var physicsSystem = new PhysicsSystem();
            AddSystem(physicsSystem);
            AddSystem(new PhysicsSyncSystem(physicsSystem.GetSimulation()));
            AddSystem(new PhysicsMouseObjectDetectSystem(physicsSystem.GetSimulation()));

            //Lighting
            AddSystem(new LightingRenderSystem(graphicsDevice));
            
            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice));
            
#if EDITOR
            AddSystem(new DebugUIRenderSystem(game, graphicsDevice, physicsSystem));
#endif

            World = Build();
            EntityFactory.World = World;
        }
    }
}
