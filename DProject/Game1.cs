using DProject.Manager;
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

        public static int ScreenResolutionX;
        public static int ScreenResolutionY;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            _entityManager = new EntityManager();
            _uiManager = new UIManager(_entityManager);
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ScreenResolutionX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenResolutionY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.PreferredBackBufferWidth = ScreenResolutionX;
            _graphics.PreferredBackBufferHeight = ScreenResolutionY;
            _graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _entityManager.Update(gameTime);
            _uiManager.Update(gameTime);
            
            base.Update(gameTime);

            PreviousKeyboardState = Keyboard.GetState();
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
        
        public static float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(sphere);
        }
        
        public static bool Intersects(Vector2 mouseLocation, Model model, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            for (int index = 0; index < model.Meshes.Count; index++)
            {
                BoundingSphere sphere = model.Meshes[index].BoundingSphere;
                sphere = sphere.Transform(world);
                float? distance = IntersectDistance(sphere, mouseLocation, view, projection, viewport);
 
                if (distance != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}