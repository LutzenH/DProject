using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity
{
    public class CameraEntity : AbstractEntity, IInitialize
    {    
        //Vectors
        private Vector3 cameraTarget;
        
        //Matrix
        public Matrix ProjectMatrix;
        public Matrix ViewMatrix;

        public CameraEntity(Vector3 position, Vector3 cameraTarget) : base(position)
        {
            this.cameraTarget = cameraTarget;
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            cameraTarget = new Vector3(0f,0f,0f);

            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                graphicsDevice.DisplayMode.AspectRatio,
                1f,
                1000f);
            
            ViewMatrix = Matrix.CreateLookAt(Position, cameraTarget, Vector3.Up);
        }
        
        public override void LoadContent(ContentManager content)
        {
            //TODO since CameraEntity uses Initialize maybe try to use LoadContent instead to remove the Initialize.
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Position.X -= 1f;
                cameraTarget.X -= 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Position.X += 1f;
                cameraTarget.X += 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Position.Y -= 1f;
                cameraTarget.Y -= 1f;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Position.Y += 1f;
                cameraTarget.Y += 1f;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                Position.Z += 1f;
                cameraTarget.Z += 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                Position.Z -= 1f;
                cameraTarget.Z -= 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
                Position = Vector3.Transform(Position, rotationMatrix);
            }

            ViewMatrix = Matrix.CreateLookAt(Position, cameraTarget, Vector3.Up);
        }
    }
}