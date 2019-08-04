using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class PropEffect : AbstractEffect, IReflected
    {
        public PropEffect(Effect cloneSource) : base(cloneSource) { }

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
    }
} 
