using DProject.Entity.Interface;
using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public class Sprite : IInitialize
    {
        public bool Visible { get; set; }
        
        public Vector2 Origin { get; set; }

        public float Rotation { get; set; }
        
        public Color Color { get; set; }

        public Rectangle Rectangle { get; set; }

        private readonly Rectangle _sourceRectangle;
        private readonly Rectangle _destinationRectangle;
        
        private readonly string _atlasName;
        private Texture2D _spriteSheet;

        public Sprite(Point position, string atlasName, string name)
        {
            _sourceRectangle = Textures.AtlasList[atlasName].TextureList[name].TextureRectangle;
            _destinationRectangle = new Rectangle(position, _sourceRectangle.Size);
            Origin = new Vector2(_sourceRectangle.Width / 2f, _sourceRectangle.Height / 2f);
            Rotation = 0.0f;
            Color = Color.White;
            Visible = true;

            _atlasName = atlasName;
            
            Rectangle = new Rectangle(new Point(_destinationRectangle.X - (int) Origin.X, _destinationRectangle.Y - (int) Origin.Y), _destinationRectangle.Size);
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _spriteSheet = Textures.AtlasList[_atlasName].AtlasTexture2D;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(Visible)
                spriteBatch.Draw(_spriteSheet, _destinationRectangle, _sourceRectangle, Color, Rotation, Origin, SpriteEffects.None, 0f);
        }
    }
} 
