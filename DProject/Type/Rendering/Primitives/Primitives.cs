using System;
using DProject.Manager.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Primitives
{
    public enum PrimitiveType { Sphere, Cube, CompanionCube }

    public class Primitives
    {
        private static Primitives _instance;
        
        private GraphicsDevice _graphicsDevice;
        
        private DPModel _sphere;
        private DPModel _cube;
        private DPModel _companionCube;

        private Primitives() { }
        
        public static Primitives Instance
        {
            get => _instance ?? (_instance = new Primitives());
            set => _instance = value;
        }
        
        public void LoadPrimitives(ContentManager contentManager)
        {
            _sphere = ModelLoaderSystem.ConvertDProjectModelFormatToModel("models/primitives/sphere", _graphicsDevice);
            _cube = ModelLoaderSystem.ConvertDProjectModelFormatToModel("models/primitives/cube", _graphicsDevice);
            _companionCube = ModelLoaderSystem.ConvertDProjectModelFormatToModel("models/primitives/companion_cube", _graphicsDevice);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Draw(Effect effect, PrimitiveType type)
        {
            int primitiveCount;
            int vertexCount;
            
            switch (type)
            {
                case PrimitiveType.Sphere:
                    _graphicsDevice.SetVertexBuffer(_sphere.VertexBuffer);
                    _graphicsDevice.Indices = _sphere.IndexBuffer;
                    primitiveCount = _sphere.PrimitiveCount;
                    vertexCount = _sphere.VertexBuffer.VertexCount;
                    break;
                case PrimitiveType.Cube:
                    _graphicsDevice.SetVertexBuffer(_cube.VertexBuffer);
                    _graphicsDevice.Indices = _cube.IndexBuffer;
                    primitiveCount = _cube.PrimitiveCount;
                    vertexCount = _cube.VertexBuffer.VertexCount;
                    break;
                case PrimitiveType.CompanionCube:
                    _graphicsDevice.SetVertexBuffer(_companionCube.VertexBuffer);
                    _graphicsDevice.Indices = _companionCube.IndexBuffer;
                    primitiveCount = _companionCube.PrimitiveCount;
                    vertexCount = _companionCube.VertexBuffer.VertexCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "The given PrimitiveType does not exist.");
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0,  vertexCount, 0, primitiveCount);
            }
        }

        public DPModel GetPrimitiveModel(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Sphere:
                    return _sphere;
                case PrimitiveType.Cube:
                    return _cube;
                case PrimitiveType.CompanionCube:
                    return _companionCube;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
