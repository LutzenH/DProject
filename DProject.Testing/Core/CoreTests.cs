﻿using System;
using System.Collections.Generic;
using System.Linq;
using DProject.Game;
using DProject.List;
using DProject.Manager;
using DProject.Manager.System;
using DProject.Manager.World;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using NUnit.Framework;

namespace DProject.Testing.Core
{
    /// <summary>
    /// NUnit test class that contains all Core Tests.
    /// (These tests need to pass on every platform)
    /// </summary>
    [TestFixture]
    public abstract class CoreTests
    {
        /// <summary>
        /// Sets (possibly) used Singletons to null for keeping the tests as atomic as possible.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            CameraSystem.ActiveLens = null;
            GraphicsManager.Instance = null;
            InputManager.Instance = null;
            ShaderManager.Instance = null;
        }

        /// <summary>
        /// Changes it's graphics settings every 240th tick.
        /// This test is used to see if any of the graphics-effects fail.
        /// </summary>
        [Test]
        public void GraphicsManager_loopOverAllGraphicsSettings()
        {
            var tickCount = 0;
            var fpsList = new List<int>();

            var game = new TestGame();
            game.InitializeInjection = () =>
            {
                GraphicsManager.Instance.EnableFXAA = false;
                GraphicsManager.Instance.EnableSky = false;
                GraphicsManager.Instance.EnableVSync = false;
                GraphicsManager.Instance.EnableFullscreen = false;
                GraphicsManager.Instance.EnableMaxFps = false;
                GraphicsManager.Instance.MaxFps = 120;
                GraphicsManager.Instance.EnableSSAO = false;
                GraphicsManager.Instance.EnableLights = false;
                GraphicsManager.Instance.EnableShadows = false;

                game.UpdateGraphicsSettings();
            };

            game.UpdateInjection = gameTime =>
            {
                if (gameTime.TotalGameTime.Ticks % 20 == 0)
                {
                    var fps = game.GetFps();
                    fpsList.Add(fps);
                }

                if (gameTime.TotalGameTime.Ticks % 240 != 0) return;

                switch (tickCount)
                {
                    case 0:
                        GraphicsManager.Instance.EnableFXAA = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 1:
                        GraphicsManager.Instance.EnableFXAA = false;
                        GraphicsManager.Instance.EnableSSAO = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 2:
                        GraphicsManager.Instance.EnableSSAO = false;
                        GraphicsManager.Instance.EnableLights = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 3:
                        GraphicsManager.Instance.EnableLights = false;
                        GraphicsManager.Instance.EnableSky = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 4:
                        GraphicsManager.Instance.EnableSky = false;
                        GraphicsManager.Instance.EnableShadows = true;
                        break;
                    case 5:
                        GraphicsManager.Instance.EnableShadows = false;
                        GraphicsManager.Instance.EnableFullscreen = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 6:
                        GraphicsManager.Instance.EnableFXAA = true;
                        GraphicsManager.Instance.EnableSky = true;
                        GraphicsManager.Instance.EnableSSAO = true;
                        GraphicsManager.Instance.EnableLights = true;
                        GraphicsManager.Instance.EnableShadows = true;
                        game.UpdateGraphicsSettings();
                        break;
                    case 7:
                        GraphicsManager.Instance.EnableVSync = true;
                        GraphicsManager.Instance.EnableFullscreen = false;
                        game.UpdateGraphicsSettings();
                        break;
                    default:
                        game.Exit();
                        break;
                }

                tickCount++;
            };

            game.Run();
            
            Console.WriteLine("Lowest FPS: " + fpsList.Min());
            Console.WriteLine("Average FPS: " + fpsList.Average());
            Console.WriteLine("Highest FPS: " + fpsList.Max());
        }

        /// <summary>
        /// Loops over every model in 'Content/models/**',
        /// to check if any of them are invalid.
        /// </summary>
        [Test]
        public void ModelLoaderSystem_loadAllModelsOneByOne()
        {
            var game = new TestGame();
            var modelList = Models.ModelList.GetEnumerator();

            Entity previousProp = null;
            
            game.InitializeInjection = () =>
            {
                game.WorldBuilder = new GameWorld(game, game.Content, game.GraphicsDevice);
                game.WorldBuilder.EntityFactory.CreateFlyCamera(new Vector3(0, 0, 3));
            };

            game.UpdateInjection = gameTime =>
            {
                if (gameTime.TotalGameTime.Ticks % 60 != 0) return;

                if (previousProp != null) 
                    previousProp.Destroy();
                
                if (modelList.MoveNext())
                    previousProp = game.WorldBuilder.EntityFactory.CreateProp(new Vector3(0, 0, 0), modelList.Current.Value.AssetPath);
                else
                    game.Exit();
            };
            
            game.Run();
            modelList.Dispose();
        }
    }

    /// <summary>
    /// Subclass of Game1 that has Actions that can be overwritten.
    /// Action injections for: Initialize, LoadContent, Update and Draw.
    /// All of these injections happen after the method executions of the base class.
    /// </summary>
    /// <example>
    /// var game = new TestGame();
    /// game.InitializeInjection = () => { YourPostInitializeMethodHere(); };
    /// </example>
    internal class TestGame : Game1
    {
        public Action InitializeInjection = () => { };
        public Action LoadContentInjection = () => { };
        public Action<GameTime> UpdateInjection = gameTime => { };
        public Action<GameTime> DrawInjection = gameTime => { };
        
        protected override void Initialize()
        {
            base.Initialize();
            InitializeInjection();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            LoadContentInjection();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateInjection(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            DrawInjection(gameTime);
        }
    }
}
