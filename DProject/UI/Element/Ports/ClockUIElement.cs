 using System;
 using DProject.Entity.Ports;
 using Microsoft.Xna.Framework;

 namespace DProject.UI.Element.Ports
{
    public class ClockUIElement : AbstractUIElement
    {
        private readonly Sprite _clockSky;
        private readonly Sprite _clockFrame;
        
        public ClockUIElement(Point position) : base(position)
        {
            _clockSky = new Sprite(new Point(position.X, position.Y + 2), "ui_elements", "clock_sky");
            _clockFrame = new Sprite(new Point(position.X, position.Y), "ui_elements", "clock_frame");
            
            AddSprite(_clockSky);
            AddSprite(_clockFrame);
        }

        public void SetRotation(uint currentTime)
        {
            var relativeTime = currentTime / (float) GameTimeEntity.TicksInADay;
            
            _clockSky.Rotation = (float) (Math.PI * 2) * relativeTime + (float)Math.PI;
        }
    }
}
