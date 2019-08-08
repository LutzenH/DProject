using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity.Camera
{
    public class FlyCameraEntity : CameraEntity
    {
        private const float Speed = 80f;

        public FlyCameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation) { }
        
        public override void Update(GameTime gameTime)
        {            
            if (IsActiveCamera)
            {
                var angleX = 0f;
                var angleY = 0f;

                var moveSpeed = (float) ((Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? Speed * 2 : Speed) * gameTime.ElapsedGameTime.TotalSeconds);
            
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Position += CameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Position -= CameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Position += Vector3.Cross(Vector3.Up, CameraDirection) * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Position -= Vector3.Cross(Vector3.Up, CameraDirection) * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    Position.Y += moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    Position.Y -= moveSpeed;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    angleX -= moveSpeed*6;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    angleX += moveSpeed*6;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    angleY += moveSpeed*6;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    angleY -= moveSpeed*6;
                
                Vector3 cameraUpPerpendicular = Vector3.Cross(Vector3.Up, CameraDirection);
                cameraUpPerpendicular.Normalize();
            
                var tempDirection = Vector3.Transform(CameraDirection, Matrix.CreateFromAxisAngle(cameraUpPerpendicular, (-MathHelper.PiOver4 / 150) * angleY));
                CameraDirection = Vector3.Transform(tempDirection, Matrix.CreateFromAxisAngle(Vector3.Up,(-MathHelper.PiOver4 / 150) * angleX));
                CameraDirection.Normalize();
            
                ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
                BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);
            }
            
            base.Update(gameTime);
        }
    }
}