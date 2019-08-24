using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component.Terrain
{
    public class LoadedClipMapTerrainComponent
    {
        public VertexBuffer TileVertexBuffer { get; set; }
        public IndexBuffer TileIndexBuffer { get; set; }

        public VertexBuffer CrossVertexBuffer { get; set; }
        public IndexBuffer CrossIndexBuffer { get; set; }

        public VertexBuffer FillerVertexBuffer { get; set; }
        public IndexBuffer FillerIndexBuffer { get; set; }
        
        public VertexBuffer SeamVertexBuffer { get; set; }
        public IndexBuffer SeamIndexBuffer { get; set; }
        
        public VertexBuffer TrimVertexBuffer { get; set; }
        public IndexBuffer TrimIndexBuffer { get; set; }
    }
}
