using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class DepthEffect : AbstractEffect
    {
        public DepthEffect(Effect cloneSource) : base(cloneSource) { }

        public float NearClipPlane
        {
            get => Parameters["NearClip"].GetValueSingle();
            set => Parameters["NearClip"].SetValue(value);
        }
        
        public float FarClipPlane
        {
            get => Parameters["FarClip"].GetValueSingle();
            set => Parameters["FarClip"].SetValue(value);
        }
    }
}
