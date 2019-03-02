using System;
using System.Collections.Generic;
using DProject.Entity;
using DProject.Manager;
using DProject.Type;
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

            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                if (entity is IUpdateable updateEntity)
                    updateEntity.Update(gameTime);
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Background color
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            foreach (AbstractEntity entity in EntityManager.GetEntities())
            {
                if (entity is IDrawable drawableEntity)
                    drawableEntity.Draw(EntityManager.GetActiveCamera());
            }
            
            base.Draw(gameTime);
        }
    }
}