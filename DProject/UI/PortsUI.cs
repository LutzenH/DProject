using DProject.Entity.Interface;
using DProject.Manager;
using DProject.UI.Element.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class PortsUI : AbstractUI, IInitialize, IUpdateable
    {
        private readonly ClockUIElement _clock;
        
        public PortsUI(GameEntityManager entityManager) : base(entityManager)
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

        public void Update(GameTime gameTime)
        {
            _clock.SetRotation(EntityManager.GetGameTimeEntity().CurrentTime);
        }
    }
} 
