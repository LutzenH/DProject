using System.Collections.Generic;
using System.Linq;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class MessageUI : AbstractUI, IUpdateable, ILoadContent
    {
        private SpriteFont _spriteFont;
        private string _textString;

        private int _lineCount;
        
        private const int FontPixelSize = 19;

        private readonly LinkedList<Message> _messages;
        
        public MessageUI(EntityManager entityManager) : base(entityManager)
        {
            _textString = "";
            _messages = new LinkedList<Message>();
        }
        
        public void LoadContent(ContentManager content)
        {
            _spriteFont = content.Load<SpriteFont>("default");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_spriteFont, _textString, new Vector2(2, Game1.ScreenResolutionY - FontPixelSize * _lineCount), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            _textString = "";
            _lineCount = 0;

            if (EditorEntityManager.Messages.Count > 0)
                _messages.AddLast(EditorEntityManager.GetFirstMessage());

            foreach (var message in _messages.ToList())
            {
                if (message.GetLifeTime() > 0)
                {
                    _textString += "[" + message.GetDateTime().ToString("HH:mm:ss") + "] " + message.GetMessage() + "\n";
                    _lineCount++;
                    message.TickLifeTime();
                }
                else
                {
                    _messages.Remove(message);
                }
            }
        }
    }
}