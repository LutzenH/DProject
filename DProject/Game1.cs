﻿using DProject.Entity;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.IDrawable;
using IUpdateable = DProject.Entity.IUpdateable;

namespace DProject
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly EntityManager EntityManager;

        private KeyboardState _previousKeyboardState;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            EntityManager = new EntityManager();
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1152;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                if (entity is IInitialize initializeEntity)
                    initializeEntity.Initialize(GraphicsDevice);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                entity.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && _previousKeyboardState.IsKeyDown(Keys.Tab))
                EntityManager.SetNextCamera();

            foreach (AbstractEntity entity in EntityManager.GetEntities())
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
            
            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                if (entity is IDrawable drawableEntity)
                    drawableEntity.Draw(EntityManager.GetActiveCamera());
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
        
        public static Vector3? GetTilePosition(float[,] heightmap, Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            for (int x = 0; x < heightmap.GetLength(0)-1; x++)
            {
                for (int y = 0; y < heightmap.GetLength(1)-1; y++)
                {
                    Vector3 position = new Vector3();
                    position.X = x;
                                        
                    position.Y = (heightmap[x, y] + heightmap[x+1, y] + heightmap[x, y+1] + heightmap[x+1, y+1]) / 4;
                    position.Z = y;
                    
                    BoundingSphere tmp = new BoundingSphere(position, 0.5f);
                    
                    if (Game1.IntersectDistance(tmp, mouseLocation, view, projection, viewport) != null)
                    {
                        return position;
                    }
                }
            }
            return null;
        }
    }
}