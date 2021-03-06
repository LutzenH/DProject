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

        public Texture2D LightInfoMap
        {
            get => Parameters["LightInfoMap"].GetValueTexture2D();
            set => Parameters["LightInfoMap"].SetValue(value);
        }
        
        public Texture2D SSAOMap
        {
            get => Parameters["SSAOMap"].GetValueTexture2D();
            set => Parameters["SSAOMap"].SetValue(value);
        }
    }
}
