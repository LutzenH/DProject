using System.Collections.Generic;
using System.Linq;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class MessageUI : AbstractUI, IUpdateable
    {
        private string _textString;

        private int _lineCount;
        
        private const int FontPixelSize = 19;

        private readonly LinkedList<Message> _messages;
        
        public MessageUI(EntityManager entityManager, UIManager uiManager) : base(entityManager, uiManager)
        {
            _textString = "";
            _messages = new LinkedList<Message>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Fonts.FontList["default_fonts"]["default"].SpriteFont, _textString, new Vector2(2, Game1.ScreenResolutionY - FontPixelSize * _lineCount), Color.White);
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