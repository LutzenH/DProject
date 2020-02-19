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
        public readonly BoundingBox BoundingBox;
        
        public DPModel(string name, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, int primitiveCount, BoundingBox boundingBox)
        {
            Name = name;
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            PrimitiveCount = primitiveCount;
            BoundingBox = boundingBox;
        }
    }
}
