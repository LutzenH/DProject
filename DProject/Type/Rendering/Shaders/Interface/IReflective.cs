 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders.Interface
{
    public interface IReflective
    {
        Vector3 CameraPosition { get; set; }

        Texture2D ReflectionBuffer { get; set; }

        Texture2D RefractionBuffer { get; set; }
    }
}