using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Type.Rendering
{
    public class WaterPlane : IInitialize, IDrawable
    {
        private Vector3 _position;
        private Vector2 _size;
        
        private VertexBuffer _vertexBuffer;

        private GraphicsDevice _graphicsDevice;
        
        private Matrix _worldMatrix;
        
        public WaterPlane(Vector3 position, Vector2 size)
        {
            _position = position;
            _size = size;
            
            _worldMatrix = Matrix.CreateTranslation(position);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTextureColorNormal), 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(
                new[]
                {
                    new VertexPositionTextureColorNormal(new Vector3(_position.X, _position.Y, _position.Z + _size.Y), Vector3.Up, Color.White, Vector2.UnitY), 
                    new VertexPositionTextureColorNormal(_position, Vector3.Up, Color.White, Vector2.Zero),
                    new VertexPositionTextureColorNormal(new Vector3(_position.X + _size.X, _position.Y, _position.Z + _size.Y), Vector3.Up, Color.White, Vector2.One),
                    new VertexPositionTextureColorNormal(_position, Vector3.Up, Color.White, Vector2.Zero),
                    new VertexPositionTextureColorNormal(new Vector3(_position.X + _size.X, _position.Y, _position.Z), Vector3.Up, Color.White, Vector2.UnitX), 
                    new VertexPositionTextureColorNormal(new Vector3(_position.X + _size.X, _position.Y, _position.Z + _size.Y), Vector3.Up, Color.White, Vector2.One)
                });
        }

        public void Draw(CameraEntity activeCamera, ShaderManager shaderManager)
        {
            AbstractEffect effect = null;
            
            switch (ShaderManager.CurrentRenderTarget)
            {
                case ShaderManager.RenderTarget.Final:
                    effect = shaderManager.WaterEffect;
                    effect.World = _worldMatrix;
                    break;
                default:
                    return;
            }
            
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }
    }
}
