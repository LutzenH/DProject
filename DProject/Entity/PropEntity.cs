using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity
{
    public class PropEntity : AbstractEntity, IInitialize, IDrawable, ILoadContent
    {        
        //Models
        private Model _model;
        private readonly string _modelPath;

        private GraphicsDevice _graphicsDevice;
        
        public PropEntity(Vector3 position, Quaternion rotation, Vector3 scale, ushort id) : base(position, rotation, scale)
        {
            _modelPath = Props.PropList[id].AssetPath;
        }
        
        public PropEntity(Vector3 position, float pitch, float yaw, float roll, Vector3 scale, ushort id) : base(position, pitch, yaw, roll, scale)
        {
            _modelPath = Props.PropList[id].AssetPath;
        }

        public PropEntity(Vector3 position, ushort id) : this(position, Quaternion.Identity, Props.PropList[id].DefaultScale, id) { }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        
        public void LoadContent(ContentManager content)
        {
            _model = content.Load<Model>(_modelPath);
        }

        public Model GetModel()
        {
            return _model;
        }
        
        public void Draw(CameraEntity activeCamera, ShaderManager shaderManager)
        {
            switch (ShaderManager.CurrentRenderTarget)
            {
                case ShaderManager.RenderTarget.Depth:
                    foreach (var mesh in _model.Meshes)
                    {
                        if (activeCamera.GetBoundingFrustum().Intersects(mesh.BoundingSphere.Transform(GetWorldMatrix())))
                        {
                            shaderManager.DepthEffect.World = GetWorldMatrix();
                            DrawMesh(shaderManager.DepthEffect, mesh.MeshParts, _graphicsDevice);
                        }
                    }
                    break;
                case ShaderManager.RenderTarget.Reflection:
                case ShaderManager.RenderTarget.Refraction:
                case ShaderManager.RenderTarget.Final:
                    foreach (var mesh in _model.Meshes)
                    {
                        if (activeCamera.GetBoundingFrustum().Intersects(mesh.BoundingSphere.Transform(GetWorldMatrix())))
                        {
                            shaderManager.PropEffect.World = GetWorldMatrix();
                            DrawMesh(shaderManager.PropEffect, mesh.MeshParts, _graphicsDevice);
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        private static void DrawMesh(Effect effect, ModelMeshPartCollection meshParts, GraphicsDevice graphicsDevice)
        {
            for (int index1 = 0; index1 < meshParts.Count; ++index1)
            {
                ModelMeshPart meshPart = meshParts[index1];
                if (meshPart.PrimitiveCount > 0)
                {
                    graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    graphicsDevice.Indices = meshPart.IndexBuffer;
                    for (int index2 = 0; index2 < effect.CurrentTechnique.Passes.Count; ++index2)
                    {
                        effect.CurrentTechnique.Passes[index2].Apply();
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }
        }
    }
}