using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Camera
{
    public abstract class CameraEntity : AbstractEntity, IUpdateable
    {    
        //Vectors
        protected Vector3 CameraDirection;
        
        //Matrix
        protected Matrix ProjectMatrix;
        protected Matrix ViewMatrix;
        
        //BoundingFrustum (used for culling)
        protected BoundingFrustum BoundingFrustum;

        public bool IsActiveCamera = false;

        protected CameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation, new Vector3(1,1,1))
        {
            Position = position;
            CameraDirection = Vector3.Forward;
            CameraDirection.Normalize();
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), 16f/9f, 0.01f, 300f);
        }

        public abstract override void LoadContent(ContentManager content);

        public virtual void Update(GameTime gameTime)
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
            BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            return BoundingFrustum;
        }

        public Matrix GetProjectMatrix()
        {
            return ProjectMatrix;
        }
        
        public Matrix GetViewMatrix()
        {
            return ViewMatrix;
        }

        public Vector3 GetCameraDirection()
        {
            return CameraDirection;
        }
    }
}