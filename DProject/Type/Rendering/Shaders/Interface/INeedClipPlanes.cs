namespace DProject.Type.Rendering.Shaders.Interface
{
    public interface INeedClipPlanes
    {
        float NearClipPlane { get; set; }
        float FarClipPlane { get; set; }
    }
}