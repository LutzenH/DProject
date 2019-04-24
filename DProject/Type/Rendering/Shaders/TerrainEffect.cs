using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class TerrainEffect : BasicEffect
    {
        public TerrainEffect(GraphicsDevice device) : base(device)
        {
            
        }

        protected TerrainEffect(BasicEffect cloneSource) : base(cloneSource)
        {
            
        }
    }
}
