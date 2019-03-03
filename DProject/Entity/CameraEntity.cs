using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity
{
    public class CameraEntity : AbstractEntity, IUpdateable
    {    
        //Vectors
        private Vector3 _cameraDirection;
        
        //Matrix
        private Matrix _projectMatrix;
        private Matrix _viewMatrix;
        
        //BoundingFrustum (used for culling)
        private BoundingFrustum boundingFrustum;

        private float speed = 0.5f;

        public bool IsActiveCamera = false;

        public CameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation, new Vector3(1,1,1))
        {
            Position = position;
            _cameraDirection = Vector3.Zero - position;
            _cameraDirection.Normalize();
            
            _viewMatrix = Matrix.CreateLookAt(Position, Position + _cameraDirection, Vector3.Up);
            _projectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 16f/9f, 0.01f, 1000);
        }
        
        public override void LoadContent(ContentManager content) {}

        public void Update(GameTime gameTime)
        {
            if (IsActiveCamera)
            {
                int anglex = 0;
                int angley = 0;

                float moveSpeed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? speed * 2 : speed;
            
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Position += _cameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Position -= _cameraDirection * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Position += Vector3.Cross(Vector3.Up, _cameraDirection) * moveSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Position -= Vector3.Cross(Vector3.Up, _cameraDirection) * moveSpeed;
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
                
                Vector3 cameraUpPerpendicular = Vector3.Cross(Vector3.Up, _cameraDirection);
                cameraUpPerpendicular.Normalize();
            
                _cameraDirection = Vector3.Transform(_cameraDirection, Matrix.CreateFromAxisAngle(cameraUpPerpendicular, (-MathHelper.PiOver4 / 150) * angley));
                _cameraDirection = Vector3.Transform(_cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Up,(-MathHelper.PiOver4 / 150) * anglex));       
            
                _cameraDirection.Normalize();
            
                _viewMatrix = Matrix.CreateLookAt(Position, Position + _cameraDirection, Vector3.Up);
                boundingFrustum = new BoundingFrustum(_viewMatrix * _projectMatrix);
            }
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            return boundingFrustum;
        }

        public Matrix GetProjectMatrix()
        {
            return _projectMatrix;
        }
        
        public Matrix GetViewMatrix()
        {
            return _viewMatrix;
        }

        public Vector3 GetCameraDirection()
        {
            return _cameraDirection;
        }
    }
}