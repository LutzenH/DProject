using DProject.Entity.Interface;
using DProject.Manager;
using DProject.UI.Element.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public class PortsUI : AbstractUI, IInitialize
    {
        private readonly ClockUIElement _clock;
        
        public PortsUI(EntityManager entityManager) : base(entityManager)
        {
            _clock = new ClockUIElement(Point.Zero);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _clock.Initialize(graphicsDevice);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            _clock.Draw(spriteBatch);
        }
    }
} 
