using System.Collections.Generic;
using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.UI.Element.Ports
{
    public class WindowUIElement : AbstractUIElement, IUpdateableUIElement
    {
        private const int WindowBarHeight = 28;
        private const int WindowTitleTextYOffset = 4;
        private const int CornerGrabOffset = -5;

        private const int ButtonExitXOffset = -8;
        private const int ButtonExitYOffset = 8;
        
        private readonly ResizableSprite _backdrop;
        private readonly Sprite _cornerGrab;

        private readonly Sprite _buttonExit;
        private readonly Sprite _buttonExitPressed;

        private readonly Text _windowTitleText;

        private bool _exitButtonPressed;
        
        public Rectangle WindowBarRectangle { get; private set; }
        
        public Rectangle CornerGrabRectangle => _cornerGrab.Rectangle;

        public Rectangle ExitButtonRectangle => _buttonExit.Rectangle;
        
        public WindowUIElement(Point position, string windowTitle, Point size) : base(position)
        {
            _backdrop = new ResizableSprite(position, size, "ui_elements", "backdrop_list");
            _cornerGrab = new Sprite(new Point(position.X + CornerGrabOffset, position.Y + CornerGrabOffset) + _backdrop.Rectangle.Size, "ui_elements", "menu_corner_grab");

            _buttonExit = new Sprite(new Point(position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, position.Y + ButtonExitYOffset), "ui_elements", "window_button_exit");
            _buttonExitPressed = new Sprite(new Point(position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, position.Y + ButtonExitYOffset), "ui_elements", "window_button_exit_pressed");
            
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
            
            AddSprite(_buttonExit);
            AddSprite(_buttonExitPressed);

            ExitButtonPressed = false;
        }
        
        public void Update(Vector2 mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButton)
        {
            if (CornerGrabRectangle.Contains(mousePosition) && currentRectangleBeingDragged == null || currentRectangleBeingDragged == CornerGrabRectangle)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Size += mousePosition.ToPoint() - Game1.PreviousMouseState.Position;
                    currentRectangleBeingDragged = CornerGrabRectangle;
                }
            }
            else if (ExitButtonRectangle.Contains(mousePosition) && currentRectangleBeingDragged == null || currentRectangleBeingDragged == ExitButtonRectangle)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    currentRectangleBeingDragged = ExitButtonRectangle;
                
                ExitButtonPressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
                
                if (Mouse.GetState().LeftButton == ButtonState.Released && Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    pressedButton = (this, "button_exit");
            }
            else if (WindowBarRectangle.Contains(mousePosition) && currentRectangleBeingDragged == null || currentRectangleBeingDragged == WindowBarRectangle)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Position += mousePosition.ToPoint() - Game1.PreviousMouseState.Position;
                    currentRectangleBeingDragged = WindowBarRectangle;
                }
            }
        }

        public Point Size
        {
            get => _backdrop.Size;
            set
            {
                _backdrop.Size = value;
                _cornerGrab.Position = new Point(Position.X + CornerGrabOffset, Position.Y + CornerGrabOffset) + _backdrop.Size;
                _buttonExit.Position = new Point(Position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, Position.Y + ButtonExitYOffset);
                _buttonExitPressed.Position = new Point(Position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, Position.Y + ButtonExitYOffset);
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
                _cornerGrab.Position = new Point(value.X + CornerGrabOffset, value.Y + CornerGrabOffset) + _backdrop.Size;
                _buttonExit.Position = new Point(Position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, Position.Y + ButtonExitYOffset);
                _buttonExitPressed.Position = new Point(Position.X + _backdrop.Rectangle.Size.X + ButtonExitXOffset, Position.Y + ButtonExitYOffset);
                WindowBarRectangle = new Rectangle(value, new Point(_backdrop.Rectangle.Width, WindowBarHeight));
                _windowTitleText.Bounds = WindowBarRectangle;
            }
        }
        
        public string Title
        {
            get => _windowTitleText.String;
            set => _windowTitleText.String = value;
        }

        public bool ExitButtonPressed
        {
            get => _exitButtonPressed;
            set
            {
                _exitButtonPressed = value;
                SetSpriteVisibility(!_exitButtonPressed, _exitButtonPressed);
            }
        }

        private void SetSpriteVisibility(bool? exit = null, bool? exitPressed = null, bool? cornerGrab = null)
        {
            if (exit != null) _buttonExit.Visible = (bool) exit;
            if (exitPressed != null) _buttonExitPressed.Visible = (bool) exitPressed;
            if (cornerGrab != null) _cornerGrab.Visible = (bool) cornerGrab;
        }
    }
} 
