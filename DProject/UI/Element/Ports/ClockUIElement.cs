 using Microsoft.Xna.Framework;

 namespace DProject.UI.Element.Ports
{
    public class ClockUIElement : AbstractUIElement
    {
        private int currentTime;
        
        private Sprite _clockSky;
        private Sprite _clockFrame;
        
        public ClockUIElement(Point position) : base(position, "ui_elements")
        {
            _clockSky = new Sprite(new Point(position.X + 48, position.Y + 48), "ui_elements", "clock_sky");
            _clockFrame = new Sprite(new Point(position.X + 48, position.Y + 48), "ui_elements", "clock_frame");
            
            AddSprite(_clockSky);
            AddSprite(_clockFrame);
        }
    }
}