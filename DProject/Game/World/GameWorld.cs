using DProject.Game;
using DProject.Game.Component.Terrain.ClipMap;
using DProject.Manager.System;
using DProject.Manager.System.Ports;
using DProject.Manager.System.Terrain;
using DProject.Manager.System.Terrain.ClipMap;
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
            //AddSystem(new ChunkLoaderSystem(_entityFactory));
            //AddSystem(new HeightmapLoaderSystem(graphicsDevice));
            //AddSystem(new HeightmapRenderSystem(graphicsDevice, shaderManager));

            AddSystem(new ClipMapTileLoaderSystem(graphicsDevice));
            AddSystem(new ClipMapLoaderSystem(graphicsDevice));
            AddSystem(new ClipMapRenderSystem(graphicsDevice, shaderManager));
            AddSystem(new ClipMapTerrainMeshLoaderSystem(graphicsDevice));
            AddSystem(new TerrainRenderSystem(graphicsDevice, shaderManager));
            
            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));
            
            World = Build();

            _entityFactory.World = World;
            _entityFactory.CreateGameTime();
            _entityFactory.CreateFlyCamera(new Vector3(0, 10, 0));
            
            _entityFactory.CreateProp(new Vector3(0, 0, 0), 10);

            _entityFactory.CreateWaterPlane(Vector2.Zero, new Vector2(4096, 4096));
            _entityFactory.CreateWaterPlane(new Vector2(-4096, 0), new Vector2(4096, 4096));

            _entityFactory.CreateClipMap(ClipMapType.Diffuse);

            _entityFactory.CreateTerrainEntity();
        }
    }
}
