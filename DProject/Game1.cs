using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Game
    {
        //GraphicsDeviceManager
        GraphicsDeviceManager graphics;
        
        //Vectors
        private Vector3 cameraTarget;
        private Vector3 cameraPosition;
        
        //Matrix
        private Matrix projectMatrix;
        private Matrix viewMatrix;
        private Matrix worldMatrix;
        
        //Models
        private Model factoryModel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1152;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            //Camera
            cameraTarget = new Vector3(0f,0f,0f);
            cameraPosition = new Vector3(0f,0f,-10f);

            projectMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.DisplayMode.AspectRatio,
                1f,
                1000f);
            
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(cameraTarget, Vector3.Forward, Vector3.Up);
        }

        protected override void LoadContent()
        {
            factoryModel = Content.Load<Model>("models/factory");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraPosition.X -= 1f;
                cameraTarget.X -= 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraPosition.X += 1f;
                cameraTarget.X += 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraPosition.Y -= 1f;
                cameraTarget.Y -= 1f;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraPosition.Y += 1f;
                cameraTarget.Y += 1f;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                cameraPosition.Z += 1f;
                cameraTarget.Z += 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                cameraPosition.Z -= 1f;
                cameraTarget.Z -= 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
                cameraPosition = Vector3.Transform(cameraPosition, rotationMatrix);
            }

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Background color
            GraphicsDevice.Clear(Color.DarkGray);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            foreach (ModelMesh mesh in factoryModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = viewMatrix;
                    effect.World = worldMatrix;
                    effect.Projection = projectMatrix;
                }
                mesh.Draw();
            }
            
            base.Draw(gameTime);
        }
    }
}