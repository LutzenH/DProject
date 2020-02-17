using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ViewportRenderSystem : IDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ViewportRenderer _viewportRenderer;

        public ViewportRenderSystem(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _viewportRenderer = new ViewportRenderer(512, 512);
        }

        public void Initialize(MonoGame.Extended.Entities.World world)
        {
           _viewportRenderer.Initialize(_graphicsDevice);
        }

        public void Draw(GameTime gameTime)
        {
            _viewportRenderer.Draw(gameTime);
        }

        public void Dispose()
        {
            _viewportRenderer.Dispose();
        }

        public ViewportRenderer[] GetViewports()
        {
            return new ViewportRenderer[]
            {
                _viewportRenderer
            };
        }
    }
}
