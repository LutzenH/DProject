using DProject.Manager.Entity;
using DProject.Manager.UI;
using DProject.UI.Element.Ports;
using DProject.UI.Handler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private Rectangle? _currentRectangleBeingDragged;
        
        public PortsUI(GameEntityManager entityManager, UIManager uiManager) : base(entityManager, uiManager)
        {
            Clock = new ClockUIElement(new Point(48, 48));
            TimeControl = new TimeControlUIElement(new Point(155, 20));
            Compass = new CompassUIElement(new Point(48, 128));
            SeasonIndicator = new SeasonIndicatorUIElement(new Point(48, 200));
            MapButton = new MapButtonUIElement(new Point(48, 272));
            DebugWindow = new WindowUIElement(new Point(0,320), "Debug Window", new Point(200, 200));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Clock.Draw(spriteBatch);
            TimeControl.Draw(spriteBatch);
            Compass.Draw(spriteBatch);
            SeasonIndicator.Draw(spriteBatch);
            MapButton.Draw(spriteBatch);
            DebugWindow.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            Compass.SetRotation(EntityManager.GetActiveCamera().GetCameraDirection());

            var mousePosition = Game1.GetMousePosition();

            for (var i = 0; i < TimeControl.ButtonRectangles.Length; i++)
            {
                if (TimeControl.ButtonRectangles[i].Contains(mousePosition))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Released &&
                        Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        var clickedButton = (TimeControlUIElement.TimeControlOption) (i + 1);
                        
                        switch (clickedButton)
                        {
                            case TimeControlUIElement.TimeControlOption.Pause:
                                UIManager.PortsEventHandler.HandleInput(Button.TimeControlPause);
                                break;
                            case TimeControlUIElement.TimeControlOption.Play:
                                UIManager.PortsEventHandler.HandleInput(Button.TimeControlPlay);
                                break;
                            case TimeControlUIElement.TimeControlOption.Speedup:
                                UIManager.PortsEventHandler.HandleInput(Button.TimeControlSpeedUp);
                                break;
                            case TimeControlUIElement.TimeControlOption.End:
                                UIManager.PortsEventHandler.HandleInput(Button.TimeControlEnd);
                                break;
                        }
                    }
                }
            }

            if (DebugWindow.CornerGrabRectangle.Contains(mousePosition) || _currentRectangleBeingDragged == DebugWindow.CornerGrabRectangle)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    DebugWindow.Size += mousePosition.ToPoint() - Game1.PreviousMouseState.Position;
                    _currentRectangleBeingDragged= DebugWindow.CornerGrabRectangle;
                }
            }
            else if (DebugWindow.WindowBarRectangle.Contains(mousePosition) || _currentRectangleBeingDragged == DebugWindow.WindowBarRectangle)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    DebugWindow.Position += mousePosition.ToPoint() - Game1.PreviousMouseState.Position;
                    _currentRectangleBeingDragged = DebugWindow.WindowBarRectangle;
                }
            }

            MapButton.Large = MapButton.Rectangle.Contains(mousePosition);

            if (MapButton.Rectangle.Contains(mousePosition))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Released &&
                    Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                {
                    UIManager.PortsEventHandler.HandleInput(Button.MapButtonPressed);
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released &&
                Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                _currentRectangleBeingDragged = null;
            }
        }
    }
} 
