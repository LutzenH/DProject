using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class ClipMapTerrainEffect : AbstractEffect
    {
        public ClipMapTerrainEffect(Effect cloneSource) : base(cloneSource) { }
        
        public Texture2D Heightmap
        {
            get => Parameters["heightmapTexture"].GetValueTexture2D();
            set => Parameters["heightmapTexture"].SetValue(value);
        }
        
        public Vector2 TextureDimension
        {
            get => Parameters["TextureDimension"].GetValueVector2();
            set => Parameters["TextureDimension"].SetValue(value);
        }
        
        public Vector2 ClipMapOffset
        {
            get => Parameters["ClipMapOffset"].GetValueVector2();
            set => Parameters["heightmapTexture"].SetValue(value);
        }
        
        public float ClipMapScale
        {
            get => Parameters["ClipMapScale"].GetValueSingle();
            set => Parameters["ClipMapScale"].SetValue(value);
        }
    }
}
