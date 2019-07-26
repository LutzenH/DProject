using System.Collections.Generic;
using DProject.Entity.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public abstract class AbstractUIElement : IInitialize
    {
        public bool Visible { get; set; }
        
        public Point Position { get; set; }

        private readonly List<Sprite> _sprites;

        protected AbstractUIElement(Point position)
        {
            Position = position;

            Visible = true;
            
            _sprites = new List<Sprite>();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
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
