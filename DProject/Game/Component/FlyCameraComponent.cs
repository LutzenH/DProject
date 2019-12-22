using DProject.Game.Interface;

namespace DProject.Game.Component
{
    public class FlyCameraComponent : IComponent, IMovable
    {
        public float Speed { get; set; }
    }
}
