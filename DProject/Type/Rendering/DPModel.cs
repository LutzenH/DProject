using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class DPModel
    {
        public readonly string Name;
        public readonly VertexBuffer VertexBuffer;
        public readonly IndexBuffer IndexBuffer;
        public readonly int PrimitiveCount;
        public readonly BoundingSphere BoundingSphere;
        
        public DPModel(string name, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, int primitiveCount, BoundingSphere boundingSphere)
        {
            Name = name;
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            PrimitiveCount = primitiveCount;
            BoundingSphere = boundingSphere;
        }
    }
}
