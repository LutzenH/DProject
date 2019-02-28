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

        public CameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation, new Vector3(1,1,1))
        {
            Position = position;
            cameraDirection = Vector3.Zero - position;
            cameraDirection.Normalize();
            cameraUp = Vector3.Up;
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + cameraDirection, cameraUp);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 16f/9f, 0.01f, 100);
        }
        
        public override void LoadContent(ContentManager content) {}

        public override void Update(GameTime gameTime)
        {
            int anglex = 0;
            int angley = 0;
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Position += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Position -= cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Position += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Position -= Vector3.Cross(cameraUp, cameraDirection) * speed;
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
                
            Vector3 cameraUpPerpendicular = Vector3.Cross(Vector3.Up, cameraDirection);
            cameraUpPerpendicular.Normalize();
            
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUpPerpendicular, (-MathHelper.PiOver4 / 150) * angley));
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Up,(-MathHelper.PiOver4 / 150) * anglex));       
            
            cameraDirection.Normalize();
            
            Console.WriteLine(cameraDirection);
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + cameraDirection, Vector3.Up);
            boundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);

            //RECHT VOORUIT KIJKEN IS 0, 0, 1
            //OMHOOG KIJKEN IS        0, 1, 0
            //OMLAAG KIJKEN IS        0,-1, 0
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            return boundingFrustum;
        }
    }
}