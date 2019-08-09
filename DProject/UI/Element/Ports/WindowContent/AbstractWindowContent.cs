using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element.Ports.WindowContent
{
    public class AbstractWindowContent : IUpdateableUIElement
    {
        public Point MinimumSize { get; set; }
        public Point MaximumSize { get; set; }
        public Point PreferredSize { get; set; }

        public bool HorizontalScrollbarVisible { get; set; }
        public bool VerticalScrollbarVisible { get; set; }

        private readonly ScrollbarUIElement _horizontalScrollbar;
        private readonly ScrollbarUIElement _verticalScrollbar;

        private Rectangle _currentBounds;
        
        public AbstractWindowContent()
        {
            var tempBounds = new Rectangle(0, 0, 200, 200);
            
            MinimumSize = tempBounds.Size;
            MaximumSize = tempBounds.Size;
            PreferredSize = tempBounds.Size;
            HorizontalScrollbarVisible = true;
            VerticalScrollbarVisible = true;

            _horizontalScrollbar = new ScrollbarUIElement(tempBounds.Location, tempBounds.Width, true);
            _verticalScrollbar = new ScrollbarUIElement(tempBounds.Location, tempBounds.Height, false);
            
            Bounds = tempBounds;
        }
        
        public Rectangle Bounds {
            get => _currentBounds;
            set
            {
                var bounds = value;
                
                bounds.Width = value.Width >= MinimumSize.X ? value.Width : MinimumSize.X;
                bounds.Height = value.Height >= MinimumSize.Y ? value.Height : MinimumSize.Y;
                
                bounds.Width = value.Width <= MaximumSize.X ? value.Width : MaximumSize.X;
                bounds.Height = value.Height <= MaximumSize.Y ? value.Height : MaximumSize.Y;
                
                _currentBounds = bounds;

                _horizontalScrollbar.Position = bounds.Location;
                _verticalScrollbar.Position = bounds.Location;
                
                _horizontalScrollbar.Size = _currentBounds.Width;
                _verticalScrollbar.Size = _currentBounds.Height;
            }
        }

        public void Update(Point mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons)
        {
            if (HorizontalScrollbarVisible)
                _horizontalScrollbar.Update(mousePosition, ref currentRectangleBeingDragged, ref pressedButtons);
            
            if(VerticalScrollbarVisible)
                _verticalScrollbar.Update(mousePosition, ref currentRectangleBeingDragged, ref pressedButtons);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (HorizontalScrollbarVisible)
                _horizontalScrollbar.Draw(spriteBatch);
            
            if(VerticalScrollbarVisible)
                _verticalScrollbar.Draw(spriteBatch);
        }
    }
}
