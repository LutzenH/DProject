using DProject.Manager.Entity;
using DProject.Manager.UI;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public abstract class AbstractUI
    {
        protected readonly dynamic EntityManager;
        protected readonly dynamic UIManager;
        
        protected AbstractUI(EntityManager entityManager, UIManager uiManager)
        {
            EntityManager = entityManager;
            UIManager = uiManager;
        }
        
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
