using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class WindowUIElement : AbstractUIElement
    {
        private const int WindowBarHeight = 28;
        private const int WindowTitleTextYOffset = 4;
        private const int CornerGrabOffset = 5;
        
        private readonly ResizableSprite _backdrop;
        private readonly Sprite _cornerGrab;

        private readonly Text _windowTitleText;

        public Rectangle WindowBarRectangle { get; private set; }
        
        public WindowUIElement(Point position, string windowTitle, Point size) : base(position)
        {
            _backdrop = new ResizableSprite(position, size, "ui_elements", "backdrop_list");
            _cornerGrab = new Sprite(new Point(position.X - CornerGrabOffset, position.Y - CornerGrabOffset) + _backdrop.Rectangle.Size, "ui_elements", "menu_corner_grab");

            WindowBarRectangle = new Rectangle(position, new Point(_backdrop.Rectangle.Width, WindowBarHeight));

            _windowTitleText = new Text(WindowBarRectangle, "written", windowTitle)
            {
                Alignment = Text.AlignmentType.Center,
                Offset = new Point(0, WindowTitleTextYOffset),
                DropShadow = true
            };
            
            Title = windowTitle;

            AddText(_windowTitleText);
            
            AddSprite(_backdrop);
            AddSprite(_cornerGrab);
        }
        
        public Rectangle CornerGrabRectangle => _cornerGrab.Rectangle;

        public Point Size
        {
            get => _backdrop.Size;
            set
            {
                _backdrop.Size = value;
                _cornerGrab.Position = new Point(Position.X - CornerGrabOffset, Position.Y - CornerGrabOffset) + _backdrop.Size;
                WindowBarRectangle = new Rectangle(Position, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
                _windowTitleText.Bounds = WindowBarRectangle;
            }
        }

        public new Point Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _backdrop.Position = value;
                _cornerGrab.Position = new Point(value.X - CornerGrabOffset, value.Y - CornerGrabOffset) + _backdrop.Size;
                WindowBarRectangle = new Rectangle(value, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
                _windowTitleText.Bounds = WindowBarRectangle;
            }
        }
        
        public string Title {
            get => _windowTitleText.String;
            set => _windowTitleText.String = value;
        }
    }
} 
