using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class SSAOEffect : Effect
    {
        public SSAOEffect(Effect cloneSource) : base(cloneSource) { }
        
        public float TotalStrength
        {
            get => Parameters["totStrength"].GetValueSingle();
            set => Parameters["totStrength"].SetValue(value);
        }
        
        public float Strength
        {
            get => Parameters["strength"].GetValueSingle();
            set => Parameters["strength"].SetValue(value);
        }
        
        public float Offset
        {
            get => Parameters["offset"].GetValueSingle();
            set => Parameters["offset"].SetValue(value);
        }
        
        public float Falloff
        {
            get => Parameters["falloff"].GetValueSingle();
            set => Parameters["falloff"].SetValue(value);
        }
        
        public float Rad
        {
            get => Parameters["rad"].GetValueSingle();
            set => Parameters["rad"].SetValue(value);
        }
        
        public Texture2D Noise
        {
            get => Parameters["NoiseMap"].GetValueTexture2D();
            set => Parameters["NoiseMap"].SetValue(value);
        }
        
        public Texture2D DepthMap
        {
            get => Parameters["DepthMap"].GetValueTexture2D();
            set => Parameters["DepthMap"].SetValue(value);
        }
        
        public Texture2D NormalMap
        {
            get => Parameters["NormalMap"].GetValueTexture2D();
            set => Parameters["NormalMap"].SetValue(value);
        }
    }
}
