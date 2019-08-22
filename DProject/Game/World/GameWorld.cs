using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Ports;
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

            //Models
            AddSystem(new ModelLoaderSystem(contentManager));
            AddSystem(new ModelRenderSystem(graphicsDevice, shaderManager));
            
            //Terrain
            AddSystem(new ChunkLoaderSystem(_entityFactory));
            AddSystem(new HeightmapLoaderSystem());
            AddSystem(new HeightmapRenderSystem(graphicsDevice, shaderManager));

            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));
            
            World = Build();

            _entityFactory.World = World;
            _entityFactory.CreateGameTime();
            _entityFactory.CreateFlyCamera();
            

            for (int x = 0; x < 25; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    _entityFactory.CreateProp(new Vector3(x * 2, 5, y * 2), (ushort) ((x*y+x+y)%11+2));
                }
            }
            
            _entityFactory.CreateWaterPlane();
        }
    }
}
