using Microsoft.Xna.Framework;

namespace DProject.Type.Rendering.Shaders.Interface
{
    public interface IReflected
    {
        Matrix ReflectionView { get; set; }

        float WaterHeight { get; set; }
    }
}