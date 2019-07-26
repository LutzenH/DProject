using System;
using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class CompassUIElement : AbstractUIElement
    {
        private readonly Sprite _compass;
        private readonly Sprite _frame;
        
        public CompassUIElement(Point position) : base(position)
        {
            _compass = new Sprite(position, "ui_elements", "compass");
            _frame = new Sprite(position, "ui_elements", "frame_simple");
           
            AddSprite(_compass);
            AddSprite(_frame);
        }
        
        public void SetRotation(Vector3 cameraDirection)
        {
            _compass.Rotation = (float) (Math.Atan2(cameraDirection.X, cameraDirection.Z) + Math.PI);
        }
    }
} 
