using System;
using DProject.Game;
using DProject.List;
using DProject.Manager;
using DProject.Manager.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const string RootDirectory = "Content/";
        
        public readonly GraphicsDeviceManager Graphics;
        public GameWorld WorldBuilder;
        
        private SpriteBatch _spriteBatch;
        
        public static int ScreenResolutionX;
        public static int ScreenResolutionY;

        private DateTime _last = DateTime.Now;
        private int _fps;
        private int _lastRecordedFps;
        
        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = RootDirectory;
            
            ScreenResolutionX = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/1.5);
            ScreenResolutionY = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/1.5);

            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.PreferredBackBufferWidth = ScreenResolutionX;
            Graphics.PreferredBackBufferHeight = ScreenResolutionY;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (sender, args) => SetScreenResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
            Window.ClientSizeChanged += (sender, args) => ShaderManager.Instance.CreateGBuffer(true);
            
            //Mouse
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Textures.Initialize(GraphicsDevice);
            
            if(WorldBuilder == null)
                WorldBuilder = new DefaultGameWorld(this, Content, GraphicsDevice);
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            ShaderManager.Instance.Initialize(GraphicsDevice);
            
            WorldBuilder.World.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Fonts.LoadFonts(Content);
            
            ShaderManager.Instance.LoadContent(Content);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();
            
            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                _lastRecordedFps = _fps;
                var title = string.Concat("DProject | FPS:", _lastRecordedFps);
                Window.Title = title;
                _fps = 0;
                _last = DateTime.Now;
            }

            WorldBuilder.World.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            WorldBuilder.World.Draw(gameTime);
            
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
                    GraphicsManager.Instance.EnableFXAA ? ShaderManager.Instance.FXAAEffect : null,
                    Matrix.Identity);

                _spriteBatch.Draw(ShaderManager.Instance.CombineFinal, GraphicsDevice.Viewport.Bounds, Color.White);
            }
            finally
            {
                _spriteBatch.End();
            }
            _fps++;
            base.Draw(gameTime);
        }

        public void SetScreenResolution(int x, int y)
        {           
            ScreenResolutionX = x;
            ScreenResolutionY = y;
            
            Graphics.PreferredBackBufferWidth = ScreenResolutionX;
            Graphics.PreferredBackBufferHeight = ScreenResolutionY;
        }

        public int GetFps()
        {
            return _lastRecordedFps;
        }

        public void UpdateGraphicsSettings()
        {
            GraphicsManager.Instance.UpdateGraphicsSettings(this);
        }
    }
}