using DProject.List;
using DProject.Manager;
using DProject.Type.Rendering;
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

        public static int WidgetOffsetX;
        public static int WidgetOffsetY;
        
        public Game1(EntityManager entityManager)
        {
            _graphics = new GraphicsDeviceManager(this);
            
            _entityManager = entityManager;
            _uiManager = new UIManager(_entityManager);
            
            Content.RootDirectory = RootDirectory;

            ScreenResolutionX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenResolutionY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            
            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;

            _backgroundColor = Color.Black;
            
            //VSYNC
            _graphics.SynchronizeWithVerticalRetrace = false;
            
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