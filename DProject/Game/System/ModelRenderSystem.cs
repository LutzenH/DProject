using DProject.Game.Component;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<LoadedModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        
        public ModelRenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(LoadedModelComponent), typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }
        
        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<LoadedModelComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            //TODO: This probably shouldn't be done in the ModelRenderSystem
            ShaderManager.Instance.SetContinuousShaderInfo(CameraSystem.ActiveLens, 0.5f);
            
            foreach (var entity in ActiveEntities)
            {
                var model = _modelMapper.Get(entity).Model;
                var transform = _transformMapper.Get(entity);

                if (model.VertexBuffer != null && model.IndexBuffer != null & model.PrimitiveCount != 0)
                {
                    if (CameraSystem.ActiveLens.BoundingFrustum.Intersects(model.BoundingSphere.Transform(transform.WorldMatrix)))
                    {
                        ShaderManager.Instance.GBufferEffect.World = transform.WorldMatrix;
                        DrawMesh(ShaderManager.Instance.GBufferEffect, model, _graphicsDevice);
                    }
                }
            }
        }

        private static void DrawMesh(Effect effect, DPModel model, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(model.VertexBuffer);
            graphicsDevice.Indices = model.IndexBuffer;
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //TODO: Maybe use vertex-offsets to loading a model, so a smaller amount of vertex-buffers have to be used.
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.PrimitiveCount);
            }
        }
    }
}
