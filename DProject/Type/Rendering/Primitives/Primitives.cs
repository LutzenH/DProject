using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Primitives
{
    public enum PrimitiveType { Sphere, Cube }

    public class Primitives
    {
        private GraphicsDevice _graphicsDevice;
        
        private (VertexBuffer, IndexBuffer, int) _sphere;
        private (VertexBuffer, IndexBuffer, int) _cube;

        public Primitives() { }

        public void LoadPrimitives(ContentManager contentManager)
        {
            //Sphere
            var sphereModel = (contentManager).Load<Model>("models/primitives/sphere");
            _sphere.Item1 = sphereModel.Meshes.First().MeshParts.First().VertexBuffer;
            _sphere.Item2 = sphereModel.Meshes.First().MeshParts.First().IndexBuffer;
            _sphere.Item3 = sphereModel.Meshes.First().MeshParts.First().PrimitiveCount;
            
            //Cube
            var cubeModel = (contentManager).Load<Model>("models/primitives/cube");
            _cube.Item1 = cubeModel.Meshes.First().MeshParts.First().VertexBuffer;
            _cube.Item2 = cubeModel.Meshes.First().MeshParts.First().IndexBuffer;
            _cube.Item3 = cubeModel.Meshes.First().MeshParts.First().PrimitiveCount;
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
