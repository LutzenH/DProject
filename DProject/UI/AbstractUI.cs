using DProject.Manager;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public abstract class AbstractUI
    {
        protected EntityManager EntityManager;
        
        protected AbstractUI(EntityManager entityManager)
        {
            EntityManager = entityManager;
        }
        
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
