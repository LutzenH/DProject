using DProject.Game.Component;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ViewportRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ViewportRenderer _viewportRenderer;

        private ComponentMapper<LoadedModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<PrimitiveComponent> _primitiveMapper;
        
        public ViewportRenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.One(typeof(LoadedModelComponent)).All(typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _viewportRenderer = new ViewportRenderer(512, 512);
        }

        public override void Initialize(IComponentMapperService mapperService)
        { 
            _modelMapper = mapperService.GetMapper<LoadedModelComponent>();
            _primitiveMapper = mapperService.GetMapper<PrimitiveComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            
            _viewportRenderer.Initialize(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var previousCullMode = _graphicsDevice.RasterizerState.CullMode;
            _viewportRenderer.Draw(gameTime);

            _graphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;
            _graphicsDevice.RasterizerState.CullMode = CullMode.None;

            foreach (var entity in ActiveEntities)
            {
                var model = _modelMapper.Get(entity).Model;
                var transform = _transformMapper.Get(entity);
                
                _viewportRenderer.DrawMesh(model, transform);
            }
            
            _graphicsDevice.RasterizerState.FillMode = FillMode.Solid;
            _graphicsDevice.RasterizerState.CullMode = previousCullMode;
        }

        ~ViewportRenderSystem()
        {
            _viewportRenderer.Dispose();
        }

        public ViewportRenderer[] GetViewports()
        {
            return new ViewportRenderer[]
            {
                _viewportRenderer
            };
        }
    }
}
