using DProject.Game.Component;
using DProject.Type.Rendering;
using DProject.Type.Rendering.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ViewportRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ViewportRenderer[] _viewportRenderers;

        private ComponentMapper<LoadedModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<PrimitiveComponent> _primitiveMapper;
        
        public ViewportRenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.One(typeof(LoadedModelComponent), typeof(PrimitiveComponent)).All(typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
            
            _viewportRenderers = new []
            {
                new ViewportRenderer(512, 512)
            };
        }

        public override void Initialize(IComponentMapperService mapperService)
        { 
            _modelMapper = mapperService.GetMapper<LoadedModelComponent>();
            _primitiveMapper = mapperService.GetMapper<PrimitiveComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            
            foreach (var viewport in _viewportRenderers)
                viewport.Initialize(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var previousCullMode = _graphicsDevice.RasterizerState.CullMode;
            _graphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;
            _graphicsDevice.RasterizerState.CullMode = CullMode.None;
            
            foreach (var viewport in _viewportRenderers)
            {
                viewport.Draw(gameTime);

                foreach (var entity in ActiveEntities)
                {
                    if (entity == DebugUIRenderSystem.SelectedEntity)
                        continue;
                    
                    if (_modelMapper.Has(entity))
                    {
                        var model = _modelMapper.Get(entity).Model;
                        var transform = _transformMapper.Get(entity);

                        viewport.DrawMesh(model, transform);
                    }
                    if (_primitiveMapper.Has(entity))
                    {
                        var primitive = _primitiveMapper.Get(entity);
                        var transform = _transformMapper.Get(entity);

                        viewport.DrawBoundingBox(transform, Primitives.Instance.GetPrimitiveModel(primitive.Type).BoundingBox, new Color(100, entity%128+128, entity%64+128));
                    }
                }
                
                if (DebugUIRenderSystem.SelectedEntity != null)
                {
                    if (_transformMapper.Has((int) DebugUIRenderSystem.SelectedEntity))
                    {
                        var transform = _transformMapper.Get((int) DebugUIRenderSystem.SelectedEntity);
                        viewport.DrawOriginMarker(transform.Position, viewport.Zoom);
                        
                        if (_modelMapper.Has((int) DebugUIRenderSystem.SelectedEntity))
                        {
                            var model = _modelMapper.Get((int) DebugUIRenderSystem.SelectedEntity);
                            viewport.DrawBoundingBox(transform, model.Model.BoundingBox);
                            
                            viewport.DrawMesh(model.Model, transform, Color.Red);
                        }
                        else if (_primitiveMapper.Has((int) DebugUIRenderSystem.SelectedEntity))
                        {
                            var primitive = _primitiveMapper.Get((int) DebugUIRenderSystem.SelectedEntity);
                            viewport.DrawBoundingBox(transform, Primitives.Instance.GetPrimitiveModel(primitive.Type).BoundingBox);
                        }
                        else
                        {
                            viewport.DrawBoundingBox(transform, new BoundingBox(
                                new Vector3(-0.5f, -0.5f, -0.5f), 
                                new Vector3(0.5f, 0.5f, 0.5f)));
                        }
                    }
                }
            }
            
            _graphicsDevice.RasterizerState.FillMode = FillMode.Solid;
            _graphicsDevice.RasterizerState.CullMode = previousCullMode;
        }

        ~ViewportRenderSystem()
        {
            foreach (var viewport in _viewportRenderers)
                viewport.Dispose();
        }

        public ViewportRenderer[] GetViewports()
        {
            return _viewportRenderers;
        }
    }
}
