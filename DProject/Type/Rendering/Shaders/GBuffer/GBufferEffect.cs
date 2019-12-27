using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class GBufferEffect : Effect, IEffectMatrices, IShadowMap
    {
        public GBufferEffect(Effect cloneSource) : base(cloneSource) { }
        
        public Matrix World
        {
            get => Parameters["World"].GetValueMatrix();
            set => Parameters["World"].SetValue(value);
        }
        
        public Matrix View
        {
            get => Parameters["View"].GetValueMatrix();
            set => Parameters["View"].SetValue(value);
        }

        public Matrix Projection {
            get => Parameters["Projection"].GetValueMatrix();
            set => Parameters["Projection"].SetValue(value);
        }

        public Texture2D ShadowMap {
            get => Parameters["ShadowMap"].GetValueTexture2D();
            set => Parameters["ShadowMap"].SetValue(value);
        }
        
        public Matrix LightView
        {
            get => Parameters["LightView"].GetValueMatrix();
            set => Parameters["LightView"].SetValue(value);
        }

        public Matrix LightProjection
        {
            get => Parameters["LightProjection"].GetValueMatrix();
            set => Parameters["LightProjection"].SetValue(value);
        }

        public float ShadowStrength
        {
            get => Parameters["ShadowStrength"].GetValueSingle();
            set => Parameters["ShadowStrength"].SetValue(value);
        }

        public float ShadowBias
        {
            get => Parameters["ShadowBias"].GetValueSingle();
            set => Parameters["ShadowBias"].SetValue(value);
        }
        
        public float ShadowMapSize
        {
            get => Parameters["ShadowMapSize"].GetValueSingle();
            set => Parameters["ShadowMapSize"].SetValue(value);
        }
    }
}
