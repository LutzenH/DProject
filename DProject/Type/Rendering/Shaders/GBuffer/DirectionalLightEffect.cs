using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class DirectionalLightEffect : Effect
    {
        public DirectionalLightEffect(Effect cloneSource) : base(cloneSource) { }

        public Vector3 LightDirection
        {
            get => Parameters["LightDirection"].GetValueVector3();
            set => Parameters["LightDirection"].SetValue(value);
        }
        
        public Vector3 LightColor
        {
            get => Parameters["LightColor"].GetValueVector3();
            set => Parameters["LightColor"].SetValue(value);
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

        public Texture2D LightInfoMap
        {
            get => Parameters["LightInfoMap"].GetValueTexture2D();
            set => Parameters["LightInfoMap"].SetValue(value);
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
