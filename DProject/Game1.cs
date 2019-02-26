using System;
using DProject.Entity;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.IDrawable;

namespace DProject
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly EntityManager EntityManager;
        
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

            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                entity.Update(gameTime);
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Background color
            GraphicsDevice.Clear(Color.DarkGray);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                if (entity is IDrawable)
                {
                    if (entity is PropEntity)
                    {
                        PropEntity propEntity = (PropEntity) entity;
                        
                        foreach (ModelMesh mesh in propEntity.getModel().Meshes)
                        {
                            Matrix worldMatrix = Matrix.CreateWorld(propEntity.GetPosition(), Vector3.Forward, Vector3.Up);
                            
                            if (EntityManager.GetActiveCamera().GetBoundingFrustum().Intersects(mesh.BoundingSphere.Transform(worldMatrix)))
                            {
                                foreach (BasicEffect effect in mesh.Effects)
                                {
                                    effect.View = EntityManager.GetActiveCamera().ViewMatrix;
                                    effect.World = worldMatrix;
                                    effect.Projection = EntityManager.GetActiveCamera().ProjectMatrix;
                                }
                                mesh.Draw();
                            }
                        }
                    }
                }
            }
            
            base.Draw(gameTime);
        }
    }
}