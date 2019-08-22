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

        private readonly GraphicsDeviceManager _graphics;
        
        private GameWorld _worldBuilder;
        private readonly ShaderManager _shaderManager;
        
        private SpriteBatch _spriteBatch;

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

            _shaderManager = new ShaderManager();
            
            ScreenResolutionX = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/1.5);
            ScreenResolutionY = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/1.5);
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;

            _backgroundColor = Color.DarkGray;

            //VSYNC
            _graphics.SynchronizeWithVerticalRetrace = false;
            
            //Max FPS
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromTicks(10000000L / MaxFps);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (sender, args) => SetScreenResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
            Window.ClientSizeChanged += (sender, args) => _shaderManager.CreateBuffers(GraphicsDevice, true);
            
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
            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                _lastRecordedFps = _fps;
                var title = string.Concat("DProject | FPS:", _lastRecordedFps);
                Window.Title = title;
                _fps = 0;
                _last = DateTime.Now;
            }

            _worldBuilder.World.Update(gameTime);

            PreviousKeyboardState = Keyboard.GetState();
            PreviousMouseState = Mouse.GetState();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

#if !EDITOR
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Depth, gameTime);
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Reflection, gameTime);
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Refraction, gameTime);      
#endif
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Final, gameTime);
            
            //TODO: Make FXAA Work on DirectX based-platforms.
#if LINUX
            GraphicsDevice.SetRenderTarget(null);

            try
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, null, _shaderManager.FXAAEffect, Matrix.Identity);
                _spriteBatch.Draw(_shaderManager.PreFinalBuffer, GraphicsDevice.Viewport.Bounds, Color.White);
            }
            finally
            {
                _spriteBatch.End();
            }
#endif
            
            base.Draw(gameTime);
            
            _fps++;
        }
        
        private void DrawSceneToRenderTarget(ShaderManager.RenderTarget renderTarget, GameTime gameTime)
        {
            _shaderManager.CurrentRenderTarget = renderTarget;

            switch (_shaderManager.CurrentRenderTarget)
            {
                case ShaderManager.RenderTarget.Depth:
                    // Set the render target
                    GraphicsDevice.SetRenderTarget(_shaderManager.DepthBuffer);
                    GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                    GraphicsDevice.BlendState = BlendState.Opaque;

                    // Clear the screen
                    GraphicsDevice.Clear(Color.White);
                    break;
                case ShaderManager.RenderTarget.Reflection:
                    //Setup Shaders
                    _shaderManager.TerrainEffect.CurrentTechnique = _shaderManager.TerrainEffect.Techniques[1];
                    _shaderManager.PropEffect.CurrentTechnique = _shaderManager.PropEffect.Techniques[1];
                    
                    // Set the render target
                    GraphicsDevice.SetRenderTarget(_shaderManager.ReflectionBuffer);
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    
                    //Clear the screen
                    GraphicsDevice.Clear(_backgroundColor);
                    break;
                case ShaderManager.RenderTarget.Refraction:
                    //Setup Shaders
                    _shaderManager.TerrainEffect.CurrentTechnique = _shaderManager.TerrainEffect.Techniques[2];
                    _shaderManager.PropEffect.CurrentTechnique = _shaderManager.PropEffect.Techniques[2];
                    
                    // Set the render target
                    GraphicsDevice.SetRenderTarget(_shaderManager.RefractionBuffer);
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    
                    //Clear the screen
                    GraphicsDevice.Clear(_backgroundColor);
                    break;
                case ShaderManager.RenderTarget.Final:
                    //Setup Shaders
                    _shaderManager.TerrainEffect.CurrentTechnique = _shaderManager.TerrainEffect.Techniques[0];
                    _shaderManager.PropEffect.CurrentTechnique = _shaderManager.PropEffect.Techniques[0];

                    // Set the render target
#if LINUX
                    GraphicsDevice.SetRenderTarget(_shaderManager.PreFinalBuffer);
#else
                    GraphicsDevice.SetRenderTarget(null);
#endif
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    
                    // Clear the screen
                    GraphicsDevice.Clear(_backgroundColor);
                    break;
            }

            //Draw the scene
            _worldBuilder.World.Draw(gameTime);
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