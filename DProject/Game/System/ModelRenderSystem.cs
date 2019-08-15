using System.Linq;
using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelRenderSystem : EntityDrawSystem
    {
        private readonly ContentManager _contentManager;
        private readonly ShaderManager _shaderManager;
        
        private ComponentMapper<ModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        
        public ModelRenderSystem(ContentManager contentManager, ShaderManager shaderManager) : base(Aspect.All(typeof(ModelComponent), typeof(TransformComponent)))
        {
            _contentManager = contentManager;
            _shaderManager = shaderManager;
        }
        
        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<ModelComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _shaderManager.SetContinuousShaderInfo(CameraSystem.ActiveLens, 0.5f);
            
            foreach (var entity in ActiveEntities)
            {
                var model = _modelMapper.Get(entity);
                var transform = _transformMapper.Get(entity);
                
                //Probably shouldn't be done in the Draw method.
                if (model.Model == null)
                    model.Model = _contentManager.Load<Model>(model.ModelPath);

                foreach (var mesh in model.Model.Meshes.Where(mesh =>
                    CameraSystem.ActiveLens.BoundingFrustum.Intersects(mesh.BoundingSphere.Transform(transform.WorldMatrix))))
                {
                    _shaderManager.PropEffect.World = transform.WorldMatrix;
                    DrawMesh(_shaderManager.PropEffect, mesh.MeshParts, _contentManager.GetGraphicsDevice());
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
