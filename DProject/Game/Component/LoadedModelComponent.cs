using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component
{
    public class LoadedModelComponent : IComponent
    {
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public int PrimitiveCount { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
    }
}
