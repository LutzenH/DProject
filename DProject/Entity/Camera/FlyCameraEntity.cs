using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity.Camera
{
    public class FlyCameraEntity : CameraEntity
    {
        private const float Speed = 0.5f;

        public FlyCameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation) { }

        public override void LoadContent(ContentManager content) { }

        public override void Update(GameTime gameTime)
        {            
            if (IsActiveCamera)
            {
                int anglex = 0;
                int angley = 0;

                float moveSpeed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? Speed * 2 : Speed;
            
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Position += CameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Position -= CameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Position += Vector3.Cross(Vector3.Up, CameraDirection) * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Position -= Vector3.Cross(Vector3.Up, CameraDirection) * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    Position.Y += 0.1f;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    Position.Y -= 0.1f;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    anglex -= 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    anglex += 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    angley += 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    angley -= 10;
                
                Vector3 cameraUpPerpendicular = Vector3.Cross(Vector3.Up, CameraDirection);
                cameraUpPerpendicular.Normalize();
            
                CameraDirection = Vector3.Transform(CameraDirection, Matrix.CreateFromAxisAngle(cameraUpPerpendicular, (-MathHelper.PiOver4 / 150) * angley));
                CameraDirection = Vector3.Transform(CameraDirection, Matrix.CreateFromAxisAngle(Vector3.Up,(-MathHelper.PiOver4 / 150) * anglex));       
            
                CameraDirection.Normalize();
            
                ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
                BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);
            }
            
            base.Update(gameTime);
        }
    }
}