using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders.Interface
{
    public interface IShadowMap
    {
        Texture2D ShadowMap { get; set; }
        
        Matrix LightView { get; set; }
        
        Matrix LightProjection { get; set; }
        
        float ShadowStrength { get; set; }
        
        float ShadowBias { get; set; }
        
        float ShadowMapSize { get; set; }
    }
} 
