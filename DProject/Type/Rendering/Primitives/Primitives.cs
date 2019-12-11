using System;
using DProject.Manager.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Primitives
{
    public enum PrimitiveType { Sphere, Cube, CompanionCube }

    public class Primitives
    {
        private GraphicsDevice _graphicsDevice;
        
        private (VertexBuffer, IndexBuffer, int) _sphere;
        private (VertexBuffer, IndexBuffer, int) _cube;
        private (VertexBuffer, IndexBuffer, int) _companionCube;

        public Primitives() { }

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
            
            switch (type)
            {
                case PrimitiveType.Sphere:
                    _graphicsDevice.SetVertexBuffer(_sphere.Item1);
                    _graphicsDevice.Indices = _sphere.Item2;
                    primitiveCount = _sphere.Item3;
                    break;
                case PrimitiveType.Cube:
                    _graphicsDevice.SetVertexBuffer(_cube.Item1);
                    _graphicsDevice.Indices = _cube.Item2;
                    primitiveCount = _cube.Item3;
                    break;
                case PrimitiveType.CompanionCube:
                    _graphicsDevice.SetVertexBuffer(_companionCube.Item1);
                    _graphicsDevice.Indices = _companionCube.Item2;
                    primitiveCount = _companionCube.Item3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "The given PrimitiveType does not exist.");
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0,  primitiveCount);
            }
        }
    }
}
