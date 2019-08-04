using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class TerrainEffect : AbstractEffect, IReflected
    {
        public TerrainEffect(Effect cloneSource) : base(cloneSource) { }
        
        public Matrix ReflectionView
        {
            get => Parameters["ReflectionView"].GetValueMatrix();
            set => Parameters["ReflectionView"].SetValue(value);
        }

        public float WaterHeight
        {
            get => Parameters["WaterHeight"].GetValueSingle();
            set => Parameters["WaterHeight"].SetValue(value);
        }
        
        //Needs to be re-implemented once the shader supports the given Sky info
        /*public TerrainEffect(GraphicsDevice device) : base(device)
        {
            LightingEnabled = true;
            PreferPerPixelLighting = true;
            VertexColorEnabled = true;
            TextureEnabled = true;

            SetLightingInfo(Skies.SkyList[Skies.GetDefaultSkyId()]);
        }

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
        }*/
    }
}
