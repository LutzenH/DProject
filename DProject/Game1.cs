using System;
using DProject.List;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Game
    {
        public const string RootDirectory = "Content/";

        private readonly GraphicsDeviceManager _graphics;
        
        private readonly EntityManager _entityManager;
        private readonly UIManager _uiManager;

        private static Color _backgroundColor;
        
        public static KeyboardState PreviousKeyboardState;
        public static MouseState PreviousMouseState;

        public static int ScreenResolutionX;
        public static int ScreenResolutionY;

#if EDITOR
        public static int WidgetOffsetX;
        public static int WidgetOffsetY;
#endif
        
        private DateTime _last = DateTime.Now;
        private int _fps;
        private int _lastRecordedFPS;
        
        public Game1(EntityManager entityManager)
        {
            _graphics = new GraphicsDeviceManager(this);
            
            _entityManager = entityManager;
            _uiManager = new UIManager(_entityManager);
            
            Content.RootDirectory = RootDirectory;

            ScreenResolutionX = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/1.5);
            ScreenResolutionY = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/1.5);
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;

            _backgroundColor = Color.Black;

            //VSYNC
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (sender, args) => SetScreenResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            //Mouse
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ShaderManager.Initialize(Content, GraphicsDevice);
            Textures.Initialize(GraphicsDevice);
            
            _entityManager.Initialize(GraphicsDevice);
            _uiManager.Initialize(GraphicsDevice);
                        
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _entityManager.LoadContent(Content);
            _uiManager.LoadContent(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                _lastRecordedFPS = _fps;
                var title = string.Concat("DProject | FPS:", _lastRecordedFPS);
                Window.Title = title;
                _fps = 0;
                _last = DateTime.Now;
            }
            else
                _fps++;
            
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
            GraphicsDevice.Clear(_backgroundColor);
            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            _entityManager.Draw();
            _uiManager.Draw();
            
            base.Draw(gameTime);
        }

        public static void SetBackgroundColor(Color color)
        {
            _backgroundColor = color;
        }

        public void SetScreenResolution(int x, int y)
        {           
            ScreenResolutionX = x;
            ScreenResolutionY = y;
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;
        }

#if EDITOR
        public void SetWidgetOffset(int x, int y)
        {
            WidgetOffsetX = x;
            WidgetOffsetY = y;
        }
#endif
        public static Vector2 GetMousePosition()
        {
#if EDITOR
            return new Vector2(Mouse.GetState().X + WidgetOffsetX, Mouse.GetState().Y + WidgetOffsetY);
#else
            return Mouse.GetState().Position.ToVector2();
#endif
        }

        public EntityManager GetEntityManager()
        {
            return _entityManager;
        }

        public int GetFPS()
        {
            return _lastRecordedFPS;
        }
    }
}