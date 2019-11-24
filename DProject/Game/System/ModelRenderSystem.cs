using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelRenderSystem : EntityDrawSystem
    {
        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<LoadedModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        
        public ModelRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.All(typeof(LoadedModelComponent), typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _shaderManager = shaderManager;
        }
        
        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<LoadedModelComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            //TODO: This probably shouldn't be done in the ModelRenderSystem
            _shaderManager.SetContinuousShaderInfo(CameraSystem.ActiveLens, 0.5f);
            
            foreach (var entity in ActiveEntities)
            {
                var model = _modelMapper.Get(entity);
                var transform = _transformMapper.Get(entity);

                if (model.Model != null)
                {
                    foreach (var mesh in model.Model.Meshes)
                    {
                        if (CameraSystem.ActiveLens.BoundingFrustum.Intersects(mesh.BoundingSphere.Transform(transform.WorldMatrix)))
                        {
                            _shaderManager.GBufferEffect.World = transform.WorldMatrix;
                            DrawMesh(_shaderManager.GBufferEffect, mesh.MeshParts, _graphicsDevice);
                        }
                    }
                }
            }
        }

        private static void DrawMesh(Effect effect, ModelMeshPartCollection meshParts, GraphicsDevice graphicsDevice)
        {
            foreach (var meshPart in meshParts)
            {
                if (meshPart.PrimitiveCount > 0)
                {
                    graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    graphicsDevice.Indices = meshPart.IndexBuffer;
                    
                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }
        }
    }
}
