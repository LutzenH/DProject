using System;
using DProject.List;
using DProject.Manager.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const string RootDirectory = "Content/";

        private readonly GraphicsDeviceManager _graphics;
        
        private readonly GameWorld _worldBuilder;

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
        private int _lastRecordedFps;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = RootDirectory;

            _worldBuilder = new GameWorld(Content);
            
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
            Textures.Initialize(GraphicsDevice);
            
            _worldBuilder.World.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Fonts.LoadFonts(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                _lastRecordedFps = _fps;
                var title = string.Concat("DProject | FPS:", _lastRecordedFps);
                Window.Title = title;
                _fps = 0;
                _last = DateTime.Now;
            }
            else
                _fps++;
            
            _worldBuilder.World.Update(gameTime);

            PreviousKeyboardState = Keyboard.GetState();
            PreviousMouseState = Mouse.GetState();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            _worldBuilder.World.Draw(gameTime);
            
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

        public static Point GetPointMousePosition()
        {
#if EDITOR
            return new Point(Mouse.GetState().X + WidgetOffsetX, Mouse.GetState().Y + WidgetOffsetY);
#else
            return Mouse.GetState().Position;
#endif
        }

        public int GetFps()
        {
            return _lastRecordedFps;
        }
    }
}