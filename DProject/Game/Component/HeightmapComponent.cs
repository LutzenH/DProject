using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component
{
    public class HeightmapComponent
    {
        public Vertex[,] Heightmap { get; set; }

        public VertexBuffer RecycledVertexBuffer { get; set; }
    }
}
