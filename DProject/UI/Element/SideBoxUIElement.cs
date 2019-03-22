using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public class SideBoxUIElement
    {
        private Texture2D _spritesheet;
        private Point _position;
        private Point _size;

        public bool Visible { get; set; }

        public SideBoxUIElement(Point position, Point size, Texture2D spritesheet)
        {
            _position = position;
            _size = size;
            _spritesheet = spritesheet;
            Visible = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            
            //TopLeft
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X, _position.Y, 4, 36), new Rectangle(41,33,2,18), Color.White);
            
            //X Stretchable top mid
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X + 4, _position.Y, _size.X, 36), new Rectangle(43,33,18,18), Color.White);
            
            //TopRight
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X + 4 + _size.X, _position.Y, 12, 36), new Rectangle(74,33,6,18), Color.White);       
            
            //Y Stretchable Mid Leftside
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X, _position.Y + 36, 2, _size.Y), new Rectangle(41,51,1,16), Color.White);
            
            //Y Stretchable Mid Rightside
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X + 4 + _size.X, _position.Y + 36, 12, _size.Y), new Rectangle(74,51,6,16), Color.White);
            
            //BottomLeft
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X, _position.Y + 36 + _size.Y, 4, 36), new Rectangle(41,67,2,18), Color.White);

            //Stretchable x bottom mid
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X + 4, _position.Y + 36 + _size.Y, _size.X, 36), new Rectangle(43,67,18,18), Color.White);

            //Bottom Right
            spriteBatch.Draw(_spritesheet, new Rectangle(_position.X + 4 + _size.X, _position.Y + 36 + _size.Y, 12, 36), new Rectangle(74,67,6,17), Color.White);

        }
    }
}