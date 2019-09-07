using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class CombineFinalEffect : Effect
    {
        public CombineFinalEffect(Effect cloneSource) : base(cloneSource) { }

        public Texture2D ColorMap
        {
            get => Parameters["ColorMap"].GetValueTexture2D();
            set => Parameters["ColorMap"].SetValue(value);
        }
        
        public Texture2D LightMap
        {
            get => Parameters["LightMap"].GetValueTexture2D();
            set => Parameters["LightMap"].SetValue(value);
        }
    }
}
