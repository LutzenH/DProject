using System.Collections.Generic;
using DProject.Entity.Interface;
using DProject.Manager.Entity;
using DProject.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Manager.UI
{
    public abstract class UIManager
    {
        private readonly List<AbstractUI> _userInterfaces;

        public static bool ClickedUI;
        
        private SpriteBatch _spriteBatch;
        
        public UIManager(EntityManager entityManager)
        {
            _userInterfaces = new List<AbstractUI>();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {    
            _spriteBatch = new SpriteBatch(graphicsDevice);
            
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

        public void AddInterface(AbstractUI userInterface)
        {
            _userInterfaces.Add(userInterface);
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
