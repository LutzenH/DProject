using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class TerrainEffect : BasicEffect
    {
        public TerrainEffect(GraphicsDevice device) : base(device) { }
        
        public void SetLightingInfo(Sky info)
        {
            AmbientLightColor = info.AmbientLightColor?.Color.ToVector3() ?? Vector3.Zero;

            Game1.SetBackgroundColor(info.BackgroundColor?.Color ?? Color.Black);

            if (info.DirectionalLight0 != null)
            {
                DirectionalLight0.Enabled = true;
                DirectionalLight0.DiffuseColor = info.DirectionalLight0.DiffuseColor.Color.ToVector3();
                DirectionalLight0.SpecularColor = info.DirectionalLight0.SpecularColor.Color.ToVector3();
                DirectionalLight0.Direction = info.DirectionalLight0.Direction;
            }

            if (info.DirectionalLight1 != null)
            {
                DirectionalLight1.Enabled = true;
                DirectionalLight1.DiffuseColor = info.DirectionalLight1.DiffuseColor.Color.ToVector3();
                DirectionalLight1.SpecularColor = info.DirectionalLight1.SpecularColor.Color.ToVector3();
                DirectionalLight1.Direction = info.DirectionalLight1.Direction;
            }

            if (info.DirectionalLight2 != null)
            {
                DirectionalLight2.Enabled = true;
                DirectionalLight2.DiffuseColor = info.DirectionalLight2.DiffuseColor.Color.ToVector3();
                DirectionalLight2.SpecularColor = info.DirectionalLight2.SpecularColor.Color.ToVector3();
                DirectionalLight2.Direction = info.DirectionalLight2.Direction;
            }
            
            if (info.Fog != null)
            {
                FogEnabled = true;
                FogColor = info.Fog.Color.Color.ToVector3();
                FogStart = info.Fog.FogStart;
                FogEnd = info.Fog.FogEnd;
            }
        }
    }
}
