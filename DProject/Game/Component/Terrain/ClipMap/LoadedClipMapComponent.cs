using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component.Terrain.ClipMap
{
    public class LoadedClipMapComponent
    {
        public ClipMapType Type { get; set; }
        
        public RenderTarget2D[] ClipMap { get; set; }
        
        public Vector2 ViewportSize { get; set; }
    }
}
