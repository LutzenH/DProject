using DProject.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class FinalRenderSystem : DrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        public FinalRenderSystem(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.DepthStencilState.StencilEnable = false;
            
            try
            {
                _spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    null,
                    GraphicsManager.Instance.EnableFXAA ? ShaderManager.Instance.FXAAEffect : null,
                    Matrix.Identity);

                _spriteBatch.Draw(ShaderManager.Instance.CombineFinal, _graphicsDevice.Viewport.Bounds, Color.White);
            }
            finally
            {
                _spriteBatch.End();
            }
            
            _graphicsDevice.DepthStencilState.StencilEnable = true;
        }
    }
} 
