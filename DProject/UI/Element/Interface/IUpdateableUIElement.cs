using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Interface
{
    public interface IUpdateableUIElement
    {
        void Update(Vector2 mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButtons);
    }
}
