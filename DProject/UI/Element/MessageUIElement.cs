using System.Collections.Generic;
using System.Linq;
using DProject.List;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.Type;
using DProject.UI.Element;
using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public class MessageUIElement : AbstractUIElement, IUpdateableUIElement
    {
        private string _textString;

        private int _lineCount;
        
        private const int FontPixelSize = 19;

        private readonly LinkedList<Message> _messages;

        private readonly Text _text;
        
        public MessageUIElement(Rectangle bounds) : base(bounds.Location)
        {
            _textString = "";
            _messages = new LinkedList<Message>();
            
            _text = new Text(bounds, "default", _textString);
            
            AddText(_text);
        }

        public void Update(Vector2 mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons)
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
            
            _text.Offset = new Point(0, _lineCount* FontPixelSize);
        }
    }
}
