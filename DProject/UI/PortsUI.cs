using DProject.Entity.Interface;
using DProject.Manager;
using DProject.UI.Element.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class PortsUI : AbstractUI, IInitialize, IUpdateable
    {
        private readonly ClockUIElement _clock;
        private readonly TimeControlUIElement _timeControl;
        private readonly CompassUIElement _compass;
        
        public PortsUI(GameEntityManager entityManager) : base(entityManager)
        {
            _clock = new ClockUIElement(new Point(48, 48));
            _timeControl = new TimeControlUIElement(new Point(155, 20));
            _compass = new CompassUIElement(new Point(48, 128));
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _clock.Initialize(graphicsDevice);
            _timeControl.Initialize(graphicsDevice);
            _compass.Initialize(graphicsDevice);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            _clock.Draw(spriteBatch);
            _timeControl.Draw(spriteBatch);
            _compass.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _clock.SetRotation(EntityManager.GetGameTimeEntity().CurrentTime);
            _compass.SetRotation(EntityManager.GetActiveCamera().GetCameraDirection());

            var mousePosition = Game1.GetMousePosition();

            for (var i = 0; i < _timeControl.ButtonRectangles.Length; i++)
            {
                if (_timeControl.ButtonRectangles[i].Contains(mousePosition))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Released && Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                        _timeControl.SelectedButton = (TimeControlUIElement.TimeControlOption) (i + 1);
                }
            }
        }

        public TimeControlUIElement.TimeControlOption GetTimeControlOption()
        {
            return _timeControl.SelectedButton;
        }
    }
} 
