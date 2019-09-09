using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class SkyEffect : Effect
    {
        public SkyEffect(Effect cloneSource) : base(cloneSource) { }

        public Vector2 ViewportResolution
        {
            get => Parameters["ViewportResolution"].GetValueVector2();
            set => Parameters["ViewportResolution"].SetValue(value);
        }

        public Texture2D Depth
        {
            get => Parameters["DepthTexture"].GetValueTexture2D();
            set => Parameters["DepthTexture"].SetValue(value);
        }
    }
}
