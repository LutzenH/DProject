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

        //Graphics Effects
        private BasicEffect basicEffect;

        //Geometric data
        private VertexPositionColor[] triangleVertices;
        private VertexBuffer vertexBuffer;
        
        //Orbit
        private bool orbit;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            //Camera
            cameraTarget = new Vector3(0f,0f,0f);
            cameraPosition = new Vector3(0f,0f,-100f);

            projectMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.DisplayMode.AspectRatio,
                1f,
                1000f);
            
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(cameraTarget, Vector3.Forward, Vector3.Up);
            
            //Graphics Effects
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            
            //Triangle Test
            triangleVertices = new VertexPositionColor[3];
            triangleVertices[0] = new VertexPositionColor(new Vector3(0,20,0), Color.Red);
            triangleVertices[1] = new VertexPositionColor(new Vector3(-20,-20,0), Color.Green);
            triangleVertices[2] = new VertexPositionColor(new Vector3(20,-20,0), Color.Blue);
            
            //Sends Vertex Information to the graphics-card
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(triangleVertices);
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
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
                orbit = !orbit;

            if (orbit)
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
                cameraPosition = Vector3.Transform(cameraPosition, rotationMatrix);
            }

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Setting the Matrixes
            basicEffect.Projection = projectMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;
            
            //Background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            
            //Back face culling
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            //Draws all primitives in the drawpass
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
            }
            
            base.Draw(gameTime);
        }
    }
}