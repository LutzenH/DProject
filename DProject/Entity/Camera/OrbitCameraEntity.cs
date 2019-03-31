using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DProject.Entity.Camera
{
    public class OrbitCameraEntity : CameraEntity
    {
        private readonly AbstractEntity _entity;
        
        public OrbitCameraEntity(AbstractEntity entityToOrbit, Quaternion rotation) : base(new Vector3(0,0,0f), rotation)
        {
            _entity = entityToOrbit;
        }

        public override void LoadContent(ContentManager content) { }

        public override void Update(GameTime gameTime)
        {
            Position = _entity.GetPosition();
            
            base.Update(gameTime);
        }
    }
}