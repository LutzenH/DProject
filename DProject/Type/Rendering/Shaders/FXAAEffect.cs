using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class FXAAEffect : Effect, IEffectMatrices
    {
        public FXAAEffect(Effect cloneSource) : base(cloneSource) { }

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
        
        public Vector2 InverseViewportSize
        {
            get => Parameters["InverseViewportSize"].GetValueVector2();
            set => Parameters["InverseViewportSize"].SetValue(value);
        }

        public float SubPixelAliasingRemoval
        {
            get => Parameters["SubPixelAliasingRemoval"].GetValueSingle();
            set => Parameters["SubPixelAliasingRemoval"].SetValue(value);
        }
        
        public float EdgeThreshold
        {
            get => Parameters["EdgeThreshold"].GetValueSingle();
            set => Parameters["EdgeThreshold"].SetValue(value);
        }
        
        public float EdgeThresholdMin
        {
            get => Parameters["EdgeThresholdMin"].GetValueSingle();
            set => Parameters["EdgeThresholdMin"].SetValue(value);
        }
    }
}