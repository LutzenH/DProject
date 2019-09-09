using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Lighting;
using DProject.Manager.System.Ports;
using DProject.Manager.System.Terrain;
using Microsoft.Xna.Framework;
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
            _entityFactory = new EntityFactory();

            //Game
            AddSystem(new GameTimeSystem());
            
            //Camera
            AddSystem(new CameraSystem());
            
            AddSystem(new ClipMapTerrainMeshLoaderSystem(graphicsDevice));
            AddSystem(new TerrainRenderSystem(graphicsDevice, shaderManager));
            
            //Models
            AddSystem(new ModelLoaderSystem(contentManager));
            AddSystem(new ModelRenderSystem(graphicsDevice, shaderManager));

            //Lighting
            AddSystem(new LightingRenderSystem(graphicsDevice, shaderManager));
            
            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));

            World = Build();

            _entityFactory.World = World;
            
            _entityFactory.CreateGameTime();
            
            _entityFactory.CreateFlyCamera(new Vector3(0, 1, 5));
            
            _entityFactory.CreateProp(new Vector3(0, 35, -400), 10);
            _entityFactory.CreateProp(new Vector3(-2, 35, -400), 9);
            _entityFactory.CreateProp(new Vector3(-4, 35, -400), 8);
            _entityFactory.CreateProp(new Vector3(-6, 35, -400), 7);
            _entityFactory.CreateProp(new Vector3(-8, 35, -400), 6);

            _entityFactory.CreateDirectionalLight(Vector3.Forward + Vector3.Down, new Color(0.8f, 0.8f, 0.8f));
            _entityFactory.CreateDirectionalLight(Vector3.Left, Color.LightSkyBlue);
            
            _entityFactory.CreatePointLight(new Vector3(-0, 35, -400), Color.Red, 25f, 1f);

            _entityFactory.CreateWaterPlane(new Vector2(0, -4096), new Vector2(4096, 4096));
            _entityFactory.CreateWaterPlane(new Vector2(-4096, -4096), new Vector2(4096, 4096));
            
            _entityFactory.CreateTerrainEntity();
        }
    }
}
