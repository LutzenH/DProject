using DProject.List;
using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element.Ports.WindowContent
{
    public abstract class AbstractWindowContent : IUpdateableUIElement
    {
        public bool HorizontalScrollbarVisible { get; set; }
        public bool VerticalScrollbarVisible { get; set; }

        private readonly ScrollbarUIElement _horizontalScrollbar;
        private readonly ScrollbarUIElement _verticalScrollbar;

        private Rectangle _currentBounds;
        private Rectangle _fullBounds;
        
        public AbstractWindowContent(Point preferredSize)
        {
            var tempBounds = new Rectangle(Point.Zero, preferredSize);
            
            Bounds = tempBounds;
            
            HorizontalScrollbarVisible = true;
            VerticalScrollbarVisible = true;

            _horizontalScrollbar = new ScrollbarUIElement(
                new Point(
                    tempBounds.X,
                    tempBounds.Y + tempBounds.Height - Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop_h"].YSize),
                tempBounds.Width, 
                true);
            
            _verticalScrollbar = new ScrollbarUIElement(
                new Point(tempBounds.X + tempBounds.Width - Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop"].XSize),
                tempBounds.Height,
                false);
        }
        
        public Rectangle CurrentBounds {
            get => _currentBounds;
            set
            {
                _currentBounds = value;
                _fullBounds.Location = _currentBounds.Location;
                
                HorizontalScrollbarVisible = _currentBounds.Width < Bounds.Width;
                VerticalScrollbarVisible = _currentBounds.Height < Bounds.Height;

                _horizontalScrollbar.HandleSizeRatio = (float) _currentBounds.Width / Bounds.Width;
                _verticalScrollbar.HandleSizeRatio = (float) _currentBounds.Height / Bounds.Height;
                
                _horizontalScrollbar.Position = new Point(
                    _currentBounds.X,
                    _currentBounds.Y + _currentBounds.Height - Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop_h"].YSize);
                
                _verticalScrollbar.Position = new Point(
                    _currentBounds.X + _currentBounds.Width - Textures.AtlasList["ui_elements"].TextureList["scrollbar_backdrop"].XSize,
                    _currentBounds.Y);
                
                _horizontalScrollbar.Size = _currentBounds.Width;
                _verticalScrollbar.Size = _currentBounds.Height;

                OnCurrentBoundsChanged();
            }
        }
        
        public Rectangle Bounds {
            get => _fullBounds;
            set => _fullBounds = value;
        }

        public virtual void Update(Point mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons)
        {
            if (HorizontalScrollbarVisible)
                _horizontalScrollbar.Update(mousePosition, ref currentRectangleBeingDragged, ref pressedButtons);
            
            if(VerticalScrollbarVisible)
                _verticalScrollbar.Update(mousePosition, ref currentRectangleBeingDragged, ref pressedButtons);
        }
        
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (HorizontalScrollbarVisible)
                _horizontalScrollbar.Draw(spriteBatch);
            
            if(VerticalScrollbarVisible)
                _verticalScrollbar.Draw(spriteBatch);
        }

        public abstract void OnCurrentBoundsChanged();
    }
}
