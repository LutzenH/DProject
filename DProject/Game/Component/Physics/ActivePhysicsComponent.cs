namespace DProject.Game.Component.Physics
{
    public enum BodyType { Static, Body }

    public class ActivePhysicsComponent
    {
        public BodyType BodyType { get; set; }
        public int? ComponentHandle { get; set; }
    }
}
