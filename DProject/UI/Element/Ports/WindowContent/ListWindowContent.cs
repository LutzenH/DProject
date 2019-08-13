using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element.Ports.WindowContent
{
    public class ListWindowContent : AbstractWindowContent
    {
        private static readonly Point PreferredSize = new Point(300,300);

        private readonly ResizableSprite _itemSprite;
        
        public ListWindowContent() : base(PreferredSize)
        {
            _itemSprite = new ResizableSprite(Bounds.Location, new Point(Bounds.Width, 32), "ui_elements", "backdrop_list_item");
        }

        public override void Update(Point mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons)
        {
            base.Update(mousePosition, ref currentRectangleBeingDragged, ref pressedButtons);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            _itemSprite.Draw(spriteBatch);
        }

        public override void OnCurrentBoundsChanged()
        {
            _itemSprite.Position = CurrentBounds.Location;
            
            _itemSprite.Size = new Point(Bounds.Width, 32);
        }
    }
} 
