using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class FullscreenQuad
    {
        private readonly GraphicsDevice _graphicsDevice;

        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        
        public FullscreenQuad(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            var vertices = new[]
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };
            
            ushort[] indices = { 0, 1, 2, 2, 3, 0 };
            
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
            
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public void Draw(Effect effect)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  2);
            }
        }
    }
}
