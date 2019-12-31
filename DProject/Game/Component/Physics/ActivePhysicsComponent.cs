using Microsoft.Xna.Framework;

namespace DProject.Game.Component.Physics
{
    public class ActivePhysicsComponent : IComponent
    {
        public int? Handle { get; set; }
        
        public Vector3 PreviousPosition { get; set; }
        public Quaternion PreviousRotation { get; set; }
    }
}
