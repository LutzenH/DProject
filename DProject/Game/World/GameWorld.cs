using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Ports;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        private EntityFactory _entityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(ContentManager contentManager, ShaderManager shaderManager)
        {
            AddSystem(new GameTimeSystem());
            AddSystem(new CameraSystem());
            AddSystem(new ModelRenderSystem(contentManager, shaderManager));
            
            World = Build();
            
            _entityFactory = new EntityFactory(World);
            _entityFactory.CreateGameTime();
            _entityFactory.CreateFlyCamera();
            _entityFactory.CreateProp();
        }
    }
}
