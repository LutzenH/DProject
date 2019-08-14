using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelRenderSystem : EntityDrawSystem
    {
        private readonly ContentManager _contentManager;
        
        private ComponentMapper<ModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        
        public ModelRenderSystem(ContentManager contentManager) : base(Aspect.All(typeof(ModelComponent), typeof(TransformComponent)))
        {
            _contentManager = contentManager;
        }
        
        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<ModelComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var model = _modelMapper.Get(entity);
                var transform = _transformMapper.Get(entity);
                
                //Probably shouldn't be done in the Draw method.
                if (model.Model == null)
                    model.Model = _contentManager.Load<Model>(model.ModelPath);

                model.Model.Draw(transform.WorldMatrix, CameraSystem.ActiveLens.View, CameraSystem.ActiveLens.Projection);
            }
        }
    }
}
