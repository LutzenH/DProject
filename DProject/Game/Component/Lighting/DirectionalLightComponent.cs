using Microsoft.Xna.Framework;

namespace DProject.Game.Component.Lighting
{
    public class DirectionalLightComponent : IComponent
    {
        public Vector3 Direction { get; set; }
        public Color Color { get; set; }
    }
}
