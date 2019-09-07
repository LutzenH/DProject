using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class ClipMapTerrainEffect : GBufferEffect
    {
        public ClipMapTerrainEffect(Effect cloneSource) : base(cloneSource) { }
        
        public Texture2D Diffuse
        {
            get => Parameters["diffuseTexture"].GetValueTexture2D();
            set => Parameters["diffuseTexture"].SetValue(value);
        }
        
        public Texture2D Height
        {
            get => Parameters["heightmapTexture"].GetValueTexture2D();
            set => Parameters["heightmapTexture"].SetValue(value);
        }
        
        public Texture2D Normal
        {
            get => Parameters["normalTexture"].GetValueTexture2D();
            set => Parameters["normalTexture"].SetValue(value);
        }
        
        public Vector2 TextureDimension
        {
            get => Parameters["TextureDimension"].GetValueVector2();
            set => Parameters["TextureDimension"].SetValue(value);
        }
        
        public Vector2 ClipMapOffset
        {
            get => Parameters["ClipMapOffset"].GetValueVector2();
            set => Parameters["ClipMapOffset"].SetValue(value);
        }
        
        public float ClipMapScale
        {
            get => Parameters["ClipMapScale"].GetValueSingle();
            set => Parameters["ClipMapScale"].SetValue(value);
        }
    }
}
