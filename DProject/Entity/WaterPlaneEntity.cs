using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity
{
    public class WaterPlaneEntity : AbstractEntity, IInitialize, IDrawable
    {
        public const float WaterHeight = 70f;

        private readonly WaterPlane _waterPlane;
        
        public WaterPlaneEntity(Vector2 position, Vector2 size) : base(new Vector3(position.X, WaterHeight, position.Y), Quaternion.Identity, new Vector3(1,1,1))
        {
            _waterPlane = new WaterPlane(new Vector3(position.X, WaterHeight, position.Y), size);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _waterPlane.Initialize(graphicsDevice);
        }

        public void Draw(CameraEntity activeCamera, ShaderManager shaderManager)
        {
            _waterPlane.Draw(activeCamera, shaderManager);
        }
    }
}
