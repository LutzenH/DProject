using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Ports;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        private EntityFactory _entityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(ContentManager contentManager, ShaderManager shaderManager, GraphicsDevice graphicsDevice)
        {
            AddSystem(new GameTimeSystem());
            AddSystem(new ModelLoaderSystem(contentManager));
            AddSystem(new HeightmapLoaderSystem(graphicsDevice));
            AddSystem(new CameraSystem());
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));
            AddSystem(new ModelRenderSystem(graphicsDevice, shaderManager));
            AddSystem(new HeightmapRenderSystem(graphicsDevice, shaderManager));
            
            World = Build();
            
            _entityFactory = new EntityFactory(World);
            _entityFactory.CreateGameTime();
            _entityFactory.CreateFlyCamera();
            _entityFactory.CreateProp();
            _entityFactory.CreateWaterPlane();
            _entityFactory.CreateHeightmap();
        }
    }
}
