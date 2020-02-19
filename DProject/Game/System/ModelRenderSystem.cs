using DProject.Game;
using DProject.Game.Component;
using DProject.Type.Rendering;
using DProject.Type.Rendering.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

namespace DProject.Manager.System
{
    public class ModelRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<LoadedModelComponent> _modelMapper;
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<PrimitiveComponent> _primitiveMapper;

        public ModelRenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.One(typeof(LoadedModelComponent), typeof(PrimitiveComponent)).All(typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }
        
        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<LoadedModelComponent>();
            _primitiveMapper = mapperService.GetMapper<PrimitiveComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (GraphicsManager.Instance.EnableShadows)
            {
                //Shadow
                var lightDirection = Vector3.Forward + Vector3.Down + Vector3.Left;
                lightDirection.Normalize();
                
                var lightPosition = (-lightDirection * 100) + new Vector3(CameraSystem.ActiveLens.Position.X, 0, CameraSystem.ActiveLens.Position.Z);

                var lightView = Matrix.CreateLookAt(lightPosition,
                    lightPosition + lightDirection,
                    Vector3.Up);
                
                //TODO: Base the width and height on the view frustum of the active lens.
                var lightProjection = Matrix.CreateOrthographic(128, 128, CameraSystem.ActiveLens.NearPlaneDistance, CameraSystem.ActiveLens.FarPlaneDistance);
                
                var boundingFrustum = new BoundingFrustum(lightView * lightProjection);

                ShaderManager.Instance.ShadowMapEffect.Projection = lightProjection;
                ShaderManager.Instance.ShadowMapEffect.View = lightView;
                ShaderManager.Instance.GBufferEffect.LightProjection = lightProjection;
                ShaderManager.Instance.GBufferEffect.LightView = lightView;

                _graphicsDevice.SetRenderTarget(ShaderManager.Instance.ShadowMap);
                _graphicsDevice.Clear(Color.White);
                DrawAllModels(ShaderManager.Instance.ShadowMapEffect);
            }

            // Deferred GBuffer
            ShaderManager.Instance.SetGBuffer();
            ShaderManager.Instance.ClearGBuffer();
            _graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
            
            //TODO: This probably shouldn't be done in the ModelRenderSystem
            ShaderManager.Instance.SetContinuousShaderInfo(CameraSystem.ActiveLens, 0.5f);
            DrawAllModels(ShaderManager.Instance.GBufferEffect);
        }

        private static void DrawMesh(Effect effect, DPModel model, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(model.VertexBuffer);
            graphicsDevice.Indices = model.IndexBuffer;
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //TODO: Maybe use vertex-offsets to loading a model, so a smaller amount of vertex-buffers have to be used.
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexBuffer.VertexCount, 0, model.PrimitiveCount);
            }
        }

        private void DrawAllModels(IEffectMatrices effect)
        {
            foreach (var entity in ActiveEntities)
            {
                if (_modelMapper.Has(entity))
                {
                    var model = _modelMapper.Get(entity).Model;
                    var transform = _transformMapper.Get(entity);

                    if (model.VertexBuffer != null && model.IndexBuffer != null & model.PrimitiveCount != 0)
                    {
                        var sphere = BoundingSphere.CreateFromBoundingBox(model.BoundingBox);

                        if (CameraSystem.ActiveLens.BoundingFrustum.Intersects(sphere.Transform(transform.WorldMatrix)))
                        {
                            effect.World = transform.WorldMatrix;
                            DrawMesh((Effect) effect, model, _graphicsDevice);
                        }
                    }
                }
                else if (_primitiveMapper.Has(entity))
                {
                    var primitive = _primitiveMapper.Get(entity);
                    var transform = _transformMapper.Get(entity);
                    
                    effect.World = transform.WorldMatrix;
                    Primitives.Instance.Draw((Effect) effect, primitive.Type);
                }
            }
        }
    }
}
