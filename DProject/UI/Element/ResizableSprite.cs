using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public class ResizableSprite : Sprite
    {
        private readonly Rectangle[] _sourceRectangles;
        private Rectangle[] _destinationRectangles;

        private readonly Point _minimumSize;

        private Point _size;

        public ResizableSprite(Point position, Point size, string atlasName, string name) : base(position, atlasName, name)
        {
            var texture = Textures.AtlasList[atlasName].TextureList[name];

            _sourceRectangles = texture.GetBorderSourceRectangles();

            _minimumSize = texture.GetTextureSize();
            _size = _minimumSize;
            
            Color = Color.White;
            Visible = true;
            Size = size;

            Rectangle = new Rectangle(new Point(size.X - (int) Origin.X, size.Y - (int) Origin.Y), size);
        }

        public new Point Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _destinationRectangles = SetDestinationRectangles(value, _size, _sourceRectangles);
                Rectangle = new Rectangle(new Point(_size.X - (int) Origin.X, _size.Y - (int) Origin.Y), _size);
            }
        }

        public Point Size {
            get => _size;
            set
            {
                _size.X = value.X >= _minimumSize.X ? value.X : _minimumSize.X;
                _size.Y = value.Y >= _minimumSize.Y ? value.Y : _minimumSize.Y;
                
                _destinationRectangles = SetDestinationRectangles(Position, _size, _sourceRectangles);
                Rectangle = new Rectangle(new Point(_size.X - (int) Origin.X, _size.Y - (int) Origin.Y), _size);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                for (var i = 0; i < _sourceRectangles.Length; i++)
                    spriteBatch.Draw(SpriteSheet, _destinationRectangles[i], _sourceRectangles[i], Color);
            }
        }

        private static Rectangle[] SetDestinationRectangles(Point position, Point size, Rectangle[] sourceRectangles)
        {
            return new[]
            {
                //Top Left
                new Rectangle(position.X, position.Y, sourceRectangles[0].Width, sourceRectangles[0].Height), 
                
                //Top Middle
                new Rectangle(position.X + sourceRectangles[0].Width, position.Y, size.X - sourceRectangles[0].Width - sourceRectangles[2].Width, sourceRectangles[0].Height), 
                
                //Top Right
                new Rectangle(position.X + size.X - sourceRectangles[2].Width, position.Y, sourceRectangles[2].Width, sourceRectangles[2].Height), 
                
                //Middle Left
                new Rectangle(position.X, position.Y + sourceRectangles[0].Height, sourceRectangles[3].Width, size.Y - sourceRectangles[0].Height - sourceRectangles[6].Height), 
                
                //Middle
                new Rectangle(position.X + sourceRectangles[3].Width, position.Y + sourceRectangles[1].Height, size.X - sourceRectangles[3].Width - sourceRectangles[5].Width, size.Y - sourceRectangles[1].Height - sourceRectangles[7].Height), 
                
                //Middle Right
                new Rectangle(position.X + size.X - sourceRectangles[5].Width, position.Y + sourceRectangles[2].Height, sourceRectangles[5].Width, size.Y - sourceRectangles[2].Height - sourceRectangles[8].Height), 
                
                //Bottom Left
                new Rectangle(position.X, position.Y + size.Y - sourceRectangles[6].Height, sourceRectangles[6].Width, sourceRectangles[6].Height), 
                
                //Bottom Middle
                new Rectangle(position.X + sourceRectangles[6].Width, position.Y + size.Y - sourceRectangles[6].Height, size.X - sourceRectangles[6].Width - sourceRectangles[8].Width, sourceRectangles[6].Height), 
                
                //Bottom Right
                new Rectangle(position.X + size.X - sourceRectangles[8].Width, position.Y + size.Y - sourceRectangles[6].Height, sourceRectangles[8].Width, sourceRectangles[8].Height) 
            };
        }
    }
}
