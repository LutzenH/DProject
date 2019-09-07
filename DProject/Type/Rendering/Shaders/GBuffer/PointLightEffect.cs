using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class PointLightEffect : Effect, IEffectMatrices
    {
        public PointLightEffect(Effect cloneSource) : base(cloneSource) { }
        
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
        
        public Vector3 LightColor
        {
            get => Parameters["LightColor"].GetValueVector3();
            set => Parameters["LightColor"].SetValue(value);
        }
        
        public Vector3 LightPosition
        {
            get => Parameters["LightPosition"].GetValueVector3();
            set => Parameters["LightPosition"].SetValue(value);
        }
        
        public float LightRadius
        {
            get => Parameters["LightRadius"].GetValueSingle();
            set => Parameters["LightRadius"].SetValue(value);
        }
        
        public float LightIntensity // = 1.0f
        {
            get => Parameters["LightIntensity"].GetValueSingle();
            set => Parameters["LightIntensity"].SetValue(value);
        }
        
        public Vector3 CameraPosition
        {
            get => Parameters["CameraPosition"].GetValueVector3();
            set => Parameters["CameraPosition"].SetValue(value);
        }
        
        public Matrix InvertViewProjection
        {
            get => Parameters["InvertViewProjection"].GetValueMatrix();
            set => Parameters["InvertViewProjection"].SetValue(value);
        }
        
        public Texture2D ColorMap
        {
            get => Parameters["ColorMap"].GetValueTexture2D();
            set => Parameters["ColorMap"].SetValue(value);
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
