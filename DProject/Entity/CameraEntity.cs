using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity
{
    public class CameraEntity : AbstractEntity
    {    
        //Vectors
        private Vector3 cameraDirection;
        private Vector3 cameraUp;
        
        //Matrix
        public Matrix ProjectMatrix;
        public Matrix ViewMatrix;
        
        //BoundingFrustum (used for culling)
        private BoundingFrustum boundingFrustum;

        private float speed = 0.5f;

        public CameraEntity(Vector3 position) : base(position)
        {
            Position = position;
            cameraDirection = Vector3.Zero - position;
            cameraDirection.Normalize();
            cameraUp = Vector3.Up;
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + cameraDirection, cameraUp);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 16f/9f, 1, 100);
        }
        
        public override void LoadContent(ContentManager content) {}

        public override void Update(GameTime gameTime)
        {
            int anglex = 0;
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Position += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Position -= cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Position += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Position -= Vector3.Cross(cameraUp, cameraDirection) * speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                anglex -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                anglex += 10;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Position.Y += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                Position.Y -= 0.1f;
            
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) * anglex));
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), (MathHelper.PiOver4 / 100) * 0));
            
            boundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + cameraDirection, cameraUp);
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            return boundingFrustum;
        }
    }
}