using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.UI.Element.Ports;
using DProject.UI.Element.Ports.WindowContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class PortsUI : AbstractUI, IUpdateable
    {
        public ClockUIElement Clock { get; }
        public TimeControlUIElement TimeControl { get; }
        public CompassUIElement Compass { get; }
        public SeasonIndicatorUIElement SeasonIndicator { get; }
        public MapButtonUIElement MapButton { get; }
        public WindowUIElement DebugWindow { get; }

        public PortsUI(GameEntityManager entityManager, UIManager uiManager) : base(entityManager, uiManager)
        {
            Clock = new ClockUIElement(new Point(48, 48));
            TimeControl = new TimeControlUIElement(new Point(155, 20));
            Compass = new CompassUIElement(new Point(48, 128));
            SeasonIndicator = new SeasonIndicatorUIElement(new Point(48, 200));
            MapButton = new MapButtonUIElement(new Point(48, 272));

            var windowContent = new AbstractWindowContent();
            
            DebugWindow = new WindowUIElement(new Point(0,320), "Debug Window", new Point(200, 200), windowContent);

            AddUIElement(Clock);
            AddUIElement(TimeControl);
            AddUIElement(Compass);
            AddUIElement(SeasonIndicator);
            AddUIElement(MapButton);
            AddUIElement(DebugWindow);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            UIManager.PortsEventHandler.HandleInput(PressedButton);
        }
    }
} 
