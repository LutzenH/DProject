using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class ListUIElement : AbstractUIElement
    {
        private readonly ResizableSprite _backdrop;
        private readonly Sprite _cornerGrab;
        
        public ListUIElement(Point position) : base(position)
        {
            _backdrop = new ResizableSprite(position, new Point(200, 200), "ui_elements", "backdrop_list");
            _cornerGrab = new Sprite(new Point(position.X - 5, position.Y - 5) + _backdrop.Rectangle.Size, "ui_elements", "menu_corner_grab");
            
            AddSprite(_backdrop);
            AddSprite(_cornerGrab);
        }

        public Point Size
        {
            get => _backdrop.Size;
            set => _backdrop.Size = value;
        }
    }
} 
