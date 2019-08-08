using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public abstract class AbstractUIElement
    {
        public bool Visible { get; set; }
        
        public Point Position { get; set; }

        private readonly List<Sprite> _sprites;
        private readonly List<Text> _texts;

        protected AbstractUIElement(Point position)
        {
            Position = position;

            Visible = true;
            
            _sprites = new List<Sprite>();
            _texts = new List<Text>();
        }

        protected void AddSprite(Sprite sprite)
        {
            _sprites.Add(sprite);
        }

        protected void AddText(Text text)
        {
            _texts.Add(text);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            foreach (var sprite in _sprites)
                sprite.Draw(spriteBatch);
            
            foreach (var text in _texts)
                text.Draw(spriteBatch);
        }
    }

} 
