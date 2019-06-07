using DProject.Type.Serializable;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;

namespace DProject.Type.Rendering
{
    public static class LightingProperties
    {
        public static readonly LightingInfo DefaultInfo = new LightingInfo()
        {
            Fog = new Fog()
            {
                Enabled = true,
                FogStart = 120f,
                FogEnd = 160f,
                
                Color = Color.DarkGray.ToVector3(),
            },
            
            AmbientLightColor = new SerializableColor()
            {
                Name = "ambient_light_color",
                Color = Color.Black
            },
            
            BackgroundColor = new SerializableColor()
            {
                Name = "background_color",
                Color = Color.DarkGray
            },
            
            DirectionalLight0 = new DirectionalLight()
            {
                Enabled = true,
                DiffuseColor = new Vector3(1f, 1f, 1f),
                SpecularColor = Color.Black.ToVector3(),
                Direction = Vector3.Normalize(Vector3.Down)
            },
            
            DirectionalLight1 = new DirectionalLight()
            {
                Enabled = false
            },
            
            DirectionalLight2 = new DirectionalLight()
            {
                Enabled = false
            }
        };
        
        public static LightingInfo CurrentInfo = DefaultInfo;
    }
} 
