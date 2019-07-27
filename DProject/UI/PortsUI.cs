using DProject.Entity.Interface;
using DProject.Entity.Ports;
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
        private readonly SeasonIndicatorUIElement _seasonIndicator;
        
        public PortsUI(GameEntityManager entityManager) : base(entityManager)
        {
            _clock = new ClockUIElement(new Point(48, 48));
            _timeControl = new TimeControlUIElement(new Point(155, 20));
            _compass = new CompassUIElement(new Point(48, 128));
            _seasonIndicator = new SeasonIndicatorUIElement(new Point(48, 200));

            GameTimeEntity gameTimeEntity = EntityManager.GetGameTimeEntity();
            gameTimeEntity.TimeChanged += (sender, args) => _clock.SetRotation(args.CurrentTime);
            gameTimeEntity.SeasonChanged += (season, args) => _seasonIndicator.SetSeason(args.CurrentSeason);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _clock.Initialize(graphicsDevice);
            _timeControl.Initialize(graphicsDevice);
            _compass.Initialize(graphicsDevice);
            _seasonIndicator.Initialize(graphicsDevice);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            _clock.Draw(spriteBatch);
            _timeControl.Draw(spriteBatch);
            _compass.Draw(spriteBatch);
            _seasonIndicator.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
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
