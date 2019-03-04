using DProject.Entity;
using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly EntityManager _entityManager;

        private KeyboardState _previousKeyboardState;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _entityManager = new EntityManager();
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1152;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            foreach (AbstractEntity entity in _entityManager.GetEntities())
            {
                if (entity is IInitialize initializeEntity)
                    initializeEntity.Initialize(GraphicsDevice);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (AbstractEntity entity in _entityManager.GetEntities())
            {
                entity.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && _previousKeyboardState.IsKeyDown(Keys.Tab))
                _entityManager.SetNextCamera();

            foreach (AbstractEntity entity in _entityManager.GetEntities())
            {
                if (entity is IUpdateable updateEntity)
                    updateEntity.Update(gameTime);
            }

            base.Update(gameTime);

            _previousKeyboardState = Keyboard.GetState();
        }

        protected override void Draw(GameTime gameTime)
        {
            //Background color
            GraphicsDevice.Clear(Color.DarkGray);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            foreach (AbstractEntity entity in _entityManager.GetEntities())
            {
                if (entity is IDrawable drawableEntity)
                    drawableEntity.Draw(_entityManager.GetActiveCamera());
            }
            
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