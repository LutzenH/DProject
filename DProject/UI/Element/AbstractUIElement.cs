using System;
using System.Collections.Generic;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public abstract class AbstractUIElement : IInitialize
    {
        public bool Visible { get; set; }
        
        public Point Position { get; set; }
        
        public TextureAtlas SpriteSheet { get; set; }

        private readonly string _textureAtlasName;

        private readonly List<Sprite> _sprites;

        protected AbstractUIElement(Point position, string textureAtlasName)
        {
            Position = position;

            if (Textures.AtlasList.ContainsKey(textureAtlasName))
                _textureAtlasName = textureAtlasName;
            else
                throw new ArgumentException("The given texture atlas does not exist.");
            
            Visible = true;
            
            _sprites = new List<Sprite>();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            if (Textures.AtlasList[_textureAtlasName].AtlasTexture2D != null)
                    SpriteSheet = Textures.AtlasList[_textureAtlasName];
            else
                throw new ArgumentException("The given texture atlas does not have a Texture2D assigned.");

            foreach (var sprite in _sprites)
                sprite.Initialize(graphicsDevice);
        }

        protected void AddSprite(Sprite sprite)
        {
            _sprites.Add(sprite);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            foreach (var sprite in _sprites)
                sprite.Draw(spriteBatch);
        }
    }

} 
