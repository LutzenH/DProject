using System;
using DProject.List;
using DProject.Manager;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        
        private readonly EntityManager _entityManager;
        private readonly UIManager _uiManager;

        public static KeyboardState PreviousKeyboardState;
        public static MouseState PreviousMouseState;

        public static int ScreenResolutionX;
        public static int ScreenResolutionY;

        public static int WidgetOffsetX;
        public static int WidgetOffsetY;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
                        
            _entityManager = new EntityManager();
            _uiManager = new UIManager(_entityManager);
            
            Content.RootDirectory = "Content";

            ScreenResolutionX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenResolutionY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;

            //VSYNC
            _graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            _entityManager.Initialize(GraphicsDevice);
            _uiManager.Initialize(GraphicsDevice);
                        
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Textures.LoadContent(Content);
            
            _entityManager.LoadContent(Content);
            _uiManager.LoadContent(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _uiManager.Update(gameTime);
            _entityManager.Update(gameTime);
            
            PreviousKeyboardState = Keyboard.GetState();
            PreviousMouseState = Mouse.GetState();
            UIManager.ClickedUI = false;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {                
            //Background color
            GraphicsDevice.Clear(Color.DarkGray);
            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            _entityManager.Draw();
            _uiManager.Draw();
            
            base.Draw(gameTime);
        }
        
        public static Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport) {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                projection,
                view,
                Matrix.Identity);
 
            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 10.0f),
                projection,
                view,
                Matrix.Identity);
 
            Vector3 direction = nearPoint - farPoint;
            
            direction.Normalize();
 
            return new Ray(nearPoint, direction);
        }

        public void SetScreenResolution(int x, int y)
        {           
            ScreenResolutionX = x;
            ScreenResolutionY = y;
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;
        }

        public static Vector2 GetAdjustedMousePosition()
        {
            return new Vector2(Mouse.GetState().X + WidgetOffsetX, Mouse.GetState().Y + WidgetOffsetY);
        }

        public void SetWidgetOffset(int x, int y)
        {
            WidgetOffsetX = x;
            WidgetOffsetY = y;
        }

        public EntityManager GetEntityManager()
        {
            return _entityManager;
        }
    }
}