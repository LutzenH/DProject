using DProject.Entity.Interface;
using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public class Sprite : IInitialize
    {
        public Vector2 Position { get; set; }

        public Rectangle SourceRectangle { get; set; }

        public Vector2 Origin { get; set; }

        public float Rotation { get; set; }

        public float Scale { get; set; }

        public Color Color { get; set; }

        private string _atlasName;
        private Texture2D _spriteSheet;

        public Sprite(Point position, string atlasName, string name)
        {
            Position = position.ToVector2();
            SourceRectangle = Textures.AtlasList[atlasName].TextureList[name].TextureRectangle;
            Origin = new Vector2(SourceRectangle.Width / 2f, SourceRectangle.Height / 2f);
            Rotation = 0.0f;
            Scale = 1.0f;
            Color = Color.White;

            _atlasName = atlasName;
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _spriteSheet = Textures.AtlasList[_atlasName].AtlasTexture2D;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_spriteSheet, Position, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
} 
