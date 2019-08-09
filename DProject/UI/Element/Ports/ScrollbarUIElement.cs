using DProject.List;
using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.UI.Element.Ports
{
    public class ScrollbarUIElement : AbstractUIElement, IUpdateableUIElement
    {
        private readonly ResizableSprite _scrollbarBackdrop;
        private readonly ResizableSprite _scrollbarHandle;

        private readonly bool _isHorizontal;
        
        private int _size;
        private float _handleSizeRatio;
        private float _currentHandlePosition;

        private Point _draggingStartPosition;

        private Point _position;
        
        public ScrollbarUIElement(Point position, int size, bool isHorizontal) : base(position)
        {
            _size = size;
            _isHorizontal = isHorizontal;

            if (isHorizontal)
            {
                _scrollbarBackdrop = new ResizableSprite(
                    position,
                    new Point(_size, Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop_h"].YSize),
                    "ui_elements",
                    "scrollbar_backdrop_h");
            
                _scrollbarHandle = new ResizableSprite(
                    position,
                    new Point((int) (_size * HandleSizeRatio), Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle_h"].YSize),
                    "ui_elements",
                    "scrollbar_handle_h");
            }
            else
            {
                _scrollbarBackdrop = new ResizableSprite(
                    position,
                    new Point(Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop"].XSize, _size),
                    "ui_elements",
                    "scrollbar_backdrop");
            
                _scrollbarHandle = new ResizableSprite(
                    position,
                    new Point(Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle"].XSize, (int) (_size * HandleSizeRatio)),
                    "ui_elements",
                    "scrollbar_handle");
            }

            AddSprite(_scrollbarBackdrop);
            AddSprite(_scrollbarHandle);
            
            HandleSizeRatio = 0.25f;
        }

        public Rectangle Handlebar => _scrollbarHandle.Rectangle;

        public void SetHandleBarPosition(float position)
        {
            var clampedPosition = MathHelper.Clamp(position, 0.0f, 1.0f);
            
            if (_isHorizontal)
                _scrollbarHandle.Position = new Point((int) (Position.X + (_size - Handlebar.Width) * clampedPosition), Position.Y);
            else
                _scrollbarHandle.Position = new Point(Position.X, (int) (Position.Y + (_size - Handlebar.Height) * clampedPosition));
            
            _currentHandlePosition = clampedPosition;
        }

        public void SetHandleBarPosition(Point draggingStartPosition, Point mousePosition)
        {
            var mousePositionDifference = mousePosition - draggingStartPosition;
            
            if(_isHorizontal)
                SetHandleBarPosition((float)(mousePositionDifference.X)/_size);
            else
                SetHandleBarPosition((float)(mousePositionDifference.Y)/_size);
        }

        public override Point Position {
            get => _position;
            set
            {
                _position = value;

                if (_scrollbarBackdrop != null && _scrollbarHandle != null)
                {
                    if (_isHorizontal)
                        _scrollbarHandle.Position = new Point((int) (Position.X + (_size - _scrollbarHandle.Rectangle.Width) * _currentHandlePosition), Position.Y);
                    else
                        _scrollbarHandle.Position = new Point(Position.X, (int) (Position.Y + (_size - _scrollbarHandle.Rectangle.Height) * _currentHandlePosition));
                
                    _scrollbarBackdrop.Position = _position;
                }
            }
        }

        public int Size
        {
            get => _size;
            set
            {
                _size = value;

                if (_isHorizontal)
                {
                    _scrollbarBackdrop.Size = new Point(value, Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop_h"].YSize);
                    _scrollbarHandle.Size = new Point((int) (_size * _handleSizeRatio), Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle_h"].YSize);
                }
                else
                {
                    _scrollbarBackdrop.Size = new Point(Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop"].XSize, value);
                    _scrollbarHandle.Size = new Point(Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle"].XSize, (int) (_size * _handleSizeRatio));
                }
            }
        }
        
        public float HandleSizeRatio {
            get => _handleSizeRatio;
            set
            {
                var clampedRatio = MathHelper.Clamp(value, 0.0f, 1.0f);
                
                _handleSizeRatio = clampedRatio;
                    
                if(_isHorizontal)
                    _scrollbarHandle.Size = new Point((int) (_size * HandleSizeRatio), Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle_h"].YSize);
                else
                    _scrollbarHandle.Size = new Point(Textures.AtlasList["ui_elements"].TextureList["scrollbar_handle"].XSize, (int) (_size * clampedRatio));
            }
        }

        public void Update(Point mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons)
        {
            if (Handlebar.Contains(mousePosition) && currentRectangleBeingDragged == null || currentRectangleBeingDragged == Handlebar)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (Game1.PreviousMouseState.LeftButton == ButtonState.Released)
                        _draggingStartPosition = mousePosition;
                    
                    SetHandleBarPosition(_draggingStartPosition, mousePosition);
                    
                    currentRectangleBeingDragged = Handlebar;
                }
            }
        }
    }
} 
