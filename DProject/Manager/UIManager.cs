using System;
using System.Collections.Generic;
using DProject.Entity.Interface;
using DProject.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Manager
{
    public class UIManager
    {
        private readonly List<AbstractUI> _userInterfaces;

        public static bool ClickedUI;
        
        private SpriteBatch _spriteBatch;

        private GraphicsDevice _graphicsDevice;
        
        public UIManager(EntityManager entityManager)
        {
            _userInterfaces = new List<AbstractUI>();

#if EDITOR
            var editorEntityManager = (EditorEntityManager) entityManager;
            _userInterfaces.Add(new WorldEditorUI(editorEntityManager));
            _userInterfaces.Add(new MessageUI(editorEntityManager)); 
#else
            var gameEntityManager = (GameEntityManager) entityManager;
            _userInterfaces.Add(new PortsUI(gameEntityManager));
#endif
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {    
            _spriteBatch = new SpriteBatch(graphicsDevice);
            
            _graphicsDevice = graphicsDevice;
            
            foreach (var ui in _userInterfaces)
            {
                if(ui is IInitialize initializedUi)
                    initializedUi.Initialize(graphicsDevice);
            }
        }
        
        public void LoadContent(ContentManager content)
        {
            foreach (var ui in _userInterfaces)
            {
                if(ui is ILoadContent contentLoadingUi)
                    contentLoadingUi.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var ui in _userInterfaces)
            {
                if(ui is IUpdateable updateableUi)
                    updateableUi.Update(gameTime);
            }
        }

        public void Draw()
        {        
            _spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                null,
                null,
                null,
                Matrix.Identity
            );
            
            foreach (var ui in _userInterfaces)
                ui.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}