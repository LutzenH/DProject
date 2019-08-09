using System;
using DProject.List;
using DProject.Manager;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject
{
    public class Game1 : Game
    {
        public const string RootDirectory = "Content/";

        private readonly GraphicsDeviceManager _graphics;

        private readonly ShaderManager _shaderManager;
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
        private int _lastRecordedFps;
        
        public Game1(EntityManager entityManager)
        {
            _graphics = new GraphicsDeviceManager(this);
            
            _shaderManager = new ShaderManager();
            _entityManager = entityManager;
            
#if EDITOR
            _uiManager = new EditorUIManager(_entityManager);
#else
            _uiManager = new GameUIManager(_entityManager);
#endif
            
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
            Textures.Initialize(GraphicsDevice);
            
            _shaderManager.Initialize(GraphicsDevice);
            _entityManager.Initialize(GraphicsDevice);
            _uiManager.Initialize(GraphicsDevice);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Fonts.LoadFonts(Content);
            
            _shaderManager.LoadContent(Content);
            _entityManager.LoadContent(Content);
            _uiManager.LoadContent(Content);
            
            base.LoadContent();
            
            // This method is temporary until it will be replaces by a proper information handler.
            _shaderManager.SetInitiateShaderInfo(_entityManager.GetActiveCamera());
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
            
            _uiManager.Update(gameTime);
            _entityManager.Update(gameTime);
            
            PreviousKeyboardState = Keyboard.GetState();
            PreviousMouseState = Mouse.GetState();
            UIManager.ClickedUI = false;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _shaderManager.SetContinuousShaderInfo(_entityManager.GetActiveCamera(), (float) _entityManager.GetGameTimeEntity().GetRelativeTime());

#if !EDITOR
            ShaderManager.CurrentRenderTarget = ShaderManager.RenderTarget.Depth;
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Depth);
            
            ShaderManager.CurrentRenderTarget = ShaderManager.RenderTarget.Reflection;
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Reflection);
            
            ShaderManager.CurrentRenderTarget = ShaderManager.RenderTarget.Refraction;
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Refraction);      
#endif

            ShaderManager.CurrentRenderTarget = ShaderManager.RenderTarget.Final;
            DrawSceneToRenderTarget(ShaderManager.RenderTarget.Final);
            
            _uiManager.Draw();
            
            base.Draw(gameTime);
        }
        
        private void DrawSceneToRenderTarget(ShaderManager.RenderTarget target)
        {
            switch (target)
            {
                case ShaderManager.RenderTarget.Depth:
                    // Set the render target
                    GraphicsDevice.SetRenderTarget(_shaderManager.DepthBuffer);
                    GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                    GraphicsDevice.BlendState = BlendState.Opaque;

                    // Clear the screen
                    GraphicsDevice.Clear(Color.Transparent);
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

                    GraphicsDevice.SetRenderTarget(null);
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    
                    // Clear the screen
                    GraphicsDevice.Clear(_backgroundColor);
                    break;
            }

            //Draw the scene
            _entityManager.Draw(_shaderManager);
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

        public EntityManager GetEntityManager()
        {
            return _entityManager;
        }

        public ShaderManager GetShaderManager()
        {
            return _shaderManager;
        }

        public int GetFps()
        {
            return _lastRecordedFps;
        }
    }
}