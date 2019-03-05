using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public abstract class AbstractUI
    {
        protected AbstractUI() { }
        
        public abstract void LoadContent(ContentManager content);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}