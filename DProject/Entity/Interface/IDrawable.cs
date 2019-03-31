using DProject.Entity.Camera;

namespace DProject.Entity.Interface
{
    public interface IDrawable
    {
        void Draw(CameraEntity activeCamera);
    }
}