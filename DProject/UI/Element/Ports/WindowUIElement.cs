using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class WindowUIElement : AbstractUIElement
    {
        private const int WindowBarHeight = 28;
        
        private readonly ResizableSprite _backdrop;
        private readonly Sprite _cornerGrab;

        public Rectangle WindowBarRectangle { get; private set; }

        public WindowUIElement(Point position) : base(position)
        {
            _backdrop = new ResizableSprite(position, new Point(200, 200), "ui_elements", "backdrop_list");
            _cornerGrab = new Sprite(new Point(position.X - 5, position.Y - 5) + _backdrop.Rectangle.Size, "ui_elements", "menu_corner_grab");
            
            AddSprite(_backdrop);
            AddSprite(_cornerGrab);
            
            WindowBarRectangle = new Rectangle(position, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
        }
        
        public Rectangle CornerGrabRectangle => _cornerGrab.Rectangle;

        public Point Size
        {
            get => _backdrop.Size;
            set
            {
                _backdrop.Size = value;
                _cornerGrab.Position = new Point(Position.X - 5, Position.Y - 5) + _backdrop.Size;
                WindowBarRectangle = new Rectangle(Position, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
            }
        }

        public new Point Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _backdrop.Position = value;
                _cornerGrab.Position = new Point(value.X - 5, value.Y - 5) + _backdrop.Size;
                WindowBarRectangle = new Rectangle(value, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
            }
        }
    }
} 
