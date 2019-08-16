using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component
{
    public class LoadedHeightmapComponent
    {
        public Point Size { get; set; }

        public int PrimitiveCount { get; set; }

        public ushort LowestHeightValue { get; set; }
        public ushort HighestHeightValue { get; set; }
        
        public DynamicVertexBuffer VertexBuffer { get; set; }
    }
}
