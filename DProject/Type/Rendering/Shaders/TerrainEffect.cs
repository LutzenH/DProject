using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class TerrainEffect : BasicEffect
    {
        public TerrainEffect(GraphicsDevice device) : base(device) { }
        
        public void SetLightingInfo(LightingInfo info)
        {
            AmbientLightColor = info.AmbientLightColor.Color.ToVector3();

            DirectionalLight0.Enabled = info.DirectionalLight0.Enabled;
            DirectionalLight0.DiffuseColor = info.DirectionalLight0.DiffuseColor;
            DirectionalLight0.SpecularColor = info.DirectionalLight0.SpecularColor;
            DirectionalLight0.Direction = info.DirectionalLight0.Direction;

            DirectionalLight1.Enabled = info.DirectionalLight1.Enabled;
            DirectionalLight1.DiffuseColor = info.DirectionalLight1.DiffuseColor;
            DirectionalLight1.SpecularColor = info.DirectionalLight1.SpecularColor;
            DirectionalLight1.Direction = info.DirectionalLight1.Direction;
                
            DirectionalLight2.Enabled = info.DirectionalLight2.Enabled;
            DirectionalLight2.DiffuseColor = info.DirectionalLight2.DiffuseColor;
            DirectionalLight2.SpecularColor = info.DirectionalLight2.SpecularColor;
            DirectionalLight2.Direction = info.DirectionalLight2.Direction;

            FogEnabled = info.Fog.Enabled;
            FogColor = info.Fog.Color;
            FogStart = info.Fog.FogStart;
            FogEnd = info.Fog.FogEnd;
        }
    }
}
