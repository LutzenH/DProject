using System;
using DProject.List;
using DProject.Manager;
using DProject.Manager.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int MaxFps = 120;
        public const string RootDirectory = "Content/";
        
        private static bool _enableFXAA = true;

        private readonly GraphicsDeviceManager _graphics;
        
        private GameWorld _worldBuilder;
        private readonly ShaderManager _shaderManager;
        
        private SpriteBatch _spriteBatch;

        private static Color _backgroundColor;

        public static int ScreenResolutionX;
        public static int ScreenResolutionY;

        private DateTime _last = DateTime.Now;
        private int _fps;
        private int _lastRecordedFps;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = RootDirectory;

            _shaderManager = new ShaderManager();
            
            ScreenResolutionX = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/1.5);
            ScreenResolutionY = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/1.5);
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _backgroundColor = Color.DarkGray;

            //VSYNC
            _graphics.SynchronizeWithVerticalRetrace = false;
            
            //Max FPS
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromTicks(10000000L / MaxFps);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (sender, args) => SetScreenResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
            Window.ClientSizeChanged += (sender, args) => _shaderManager.CreateGBuffer(true);
            
            //Mouse
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Textures.Initialize(GraphicsDevice);
            
            _worldBuilder = new GameWorld(Content, _shaderManager, GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _shaderManager.Initialize(GraphicsDevice);
            _worldBuilder.World.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Fonts.LoadFonts(Content);
            
            _shaderManager.LoadContent(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();
            
            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                _lastRecordedFps = _fps;
                var title = string.Concat("DProject | FPS:", _lastRecordedFps);
                Window.Title = title;
                _fps = 0;
                _last = DateTime.Now;
            }

            _worldBuilder.World.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _shaderManager.SetGBuffer();
            
            _shaderManager.ClearGBuffer();
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
            
            _worldBuilder.World.Draw(gameTime);
            
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
            
            try
            {
                _spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    null,
                    _enableFXAA ? _shaderManager.FXAAEffect : null,
                    Matrix.Identity);

                _spriteBatch.Draw(_shaderManager.CombineFinal, GraphicsDevice.Viewport.Bounds, Color.White);
            }
            finally
            {
                _spriteBatch.End();
            }
            _fps++;
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
        
        public static Vector2 GetMousePosition()
        {
            return Mouse.GetState().Position.ToVector2();
        }

        public static Point GetPointMousePosition()
        {
            return Mouse.GetState().Position;
        }

        public int GetFps()
        {
            return _lastRecordedFps;
        }
    }
}