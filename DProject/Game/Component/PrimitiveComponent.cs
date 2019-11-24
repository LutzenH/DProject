using DProject.Type.Rendering.Primitives;
using Microsoft.Xna.Framework;

namespace DProject.Game.Component
{
    public class PrimitiveComponent : TransformComponent
    {
        public PrimitiveType Type { get; set; }
        public Color Color { get; set; }
    }
}
