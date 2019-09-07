namespace DProject.Game.Component.Terrain.ClipMap
{
    public enum ClipMapType { Height, Normal, Diffuse }

    public class ClipMapComponent
    {
        public ClipMapType Type { get; set; }
    }
}
