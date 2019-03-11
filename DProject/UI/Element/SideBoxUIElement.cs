using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace DProject.UI.Element
{
    public class SideBoxUIElement
    {
        private Rectangle _sideBoxRectangle;
        private Texture2D _spritesheet;
        private Vector2 _position;

        public SideBoxUIElement(Vector2 position, Rectangle sideBoxRectangle, Texture2D spritesheet)
        {
            _position = position;
            _sideBoxRectangle = sideBoxRectangle;
            _spritesheet = spritesheet;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}