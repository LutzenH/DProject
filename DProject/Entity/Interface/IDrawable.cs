using DProject.Entity.Camera;
using DProject.Manager;

namespace DProject.Entity.Interface
{
    public interface IDrawable
    {
        void Draw(CameraEntity activeCamera, ShaderManager shaderManager);
    }
}