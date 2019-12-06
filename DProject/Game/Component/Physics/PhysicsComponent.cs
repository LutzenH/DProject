using System.Numerics;
using BepuPhysics.Collidables;

namespace DProject.Game.Component.Physics
{
    public class PhysicsComponent : IComponent
    {
        public Vector3 StartPosition { get; set; }
        public IConvexShape Shape { get; set; }
        public float SpeculativeMargin { get; set; }
    }
}
