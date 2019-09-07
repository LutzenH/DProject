using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class GBufferEffect : Effect, IEffectMatrices
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

        public float SpecularIntensity
        {
            get => Parameters["SpecularIntensity"].GetValueSingle();
            set => Parameters["SpecularIntensity"].SetValue(value);
        }
        
        public float SpecularPower
        {
            get => Parameters["SpecularPower"].GetValueSingle();
            set => Parameters["SpecularPower"].SetValue(value);
        }
    }
}
