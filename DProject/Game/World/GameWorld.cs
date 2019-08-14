using DProject.Game;
using DProject.Manager.System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        private EntityFactory _entityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(ContentManager contentManager)
        {
            AddSystem(new CameraSystem());
            AddSystem(new ModelRenderSystem(contentManager));
            
            World = Build();
            
            _entityFactory = new EntityFactory(World);
            _entityFactory.CreateCamera();
            _entityFactory.CreateProp();
        }
    }
}
