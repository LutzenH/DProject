using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelLoaderSystem : EntityUpdateSystem
    {
        private readonly ContentManager _contentManager;
        
        private ComponentMapper<ModelComponent> _modelMapper;
        private ComponentMapper<LoadedModelComponent> _loadedModelMapper;

        public ModelLoaderSystem(ContentManager contentManager) : base(Aspect.All(typeof(ModelComponent)))
        {
            _contentManager = contentManager;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<ModelComponent>();
            _loadedModelMapper = mapperService.GetMapper<LoadedModelComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var modelComponent = _modelMapper.Get(entity);
                
                var loadedModelComponent = new LoadedModelComponent()
                {
                    Model = _contentManager.Load<Model>(modelComponent.ModelPath)
                };
                
                _loadedModelMapper.Put(entity, loadedModelComponent);
                _modelMapper.Delete(entity);
            }
        }
    }
}
