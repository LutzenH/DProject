using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        private EntityFactory _entityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(ContentManager contentManager, ShaderManager shaderManager, GraphicsDevice graphicsDevice)
        {
            _entityFactory = new EntityFactory();

            //Camera
            AddSystem(new CameraSystem());

            //Models
            AddSystem(new ModelLoaderSystem(contentManager));
            AddSystem(new ModelRenderSystem(graphicsDevice, shaderManager));

            //Primitives
            AddSystem(new PrimitiveRenderSystem(graphicsDevice, shaderManager));
            
            //Lighting
            AddSystem(new LightingRenderSystem(graphicsDevice, shaderManager));
            
            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));

            World = Build();
            _entityFactory.World = World;
            
            //Entities
            _entityFactory.CreateFlyCamera(new Vector3(0, 0, 0));
            _entityFactory.CreateProp(new Vector3(0, 0, 0), 10);
            _entityFactory.CreateProp(new Vector3(-2, 0, 0), 9);
            _entityFactory.CreateProp(new Vector3(-4, 0, 0), 8);
            _entityFactory.CreateProp(new Vector3(-6, 0, 0), 7);
            _entityFactory.CreateProp(new Vector3(-8, 0, 0), 6);
            
            _entityFactory.CreatePrimitive(Vector3.Zero, new Vector3(4, 4, 4), PrimitiveType.Cube);

            //Lights
            _entityFactory.CreateDirectionalLight(Vector3.Forward + Vector3.Down, new Color(0.8f, 0.8f, 0.8f));
            _entityFactory.CreateDirectionalLight(Vector3.Left, Color.LightSkyBlue);
            _entityFactory.CreatePointLight(new Vector3(-0, 2, 0), Color.Red, 25f, 1f);
        }
    }
}
