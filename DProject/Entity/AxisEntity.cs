using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Entity
{
    public class AxisEntity : AbstractEntity, IInitialize, IDrawable
    {
        private BasicEffect _basicEffect;
        
        private readonly Vector3 _lineStartPoint;
        private readonly Vector3 _lineEndPointX;
        private readonly Vector3 _lineEndPointY;
        private readonly Vector3 _lineEndPointZ;
        
        private VertexBuffer _vertexBuffer;
        
        private GraphicsDevice _graphicsDevice;
        
        public AxisEntity(Vector3 position, Quaternion rotation, Vector3 scale) : base(position, rotation, scale)
        {            
            _lineStartPoint = Vector3.Zero;
            _lineEndPointX = Vector3.Left;
            _lineEndPointY = Vector3.Forward;
            _lineEndPointZ = Vector3.Up;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.Alpha = 1.0f;
            _basicEffect.LightingEnabled = false;
            _basicEffect.VertexColorEnabled = true;
            
            //Sends Vertex Information to the graphics-card
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(
                new[]
                {
                    new VertexPositionColor(_lineStartPoint, Color.Red),  new VertexPositionColor(_lineEndPointX, Color.Red),
                    new VertexPositionColor(_lineStartPoint, Color.Green),  new VertexPositionColor(_lineEndPointY, Color.Green),
                    new VertexPositionColor(_lineStartPoint, Color.Blue),  new VertexPositionColor(_lineEndPointZ, Color.Blue)
                });
        }


        public void Draw(CameraEntity activeCamera)
        {
            _basicEffect.View = activeCamera.GetViewMatrix();
            _basicEffect.World = getWorldMatrix();
            _basicEffect.Projection = activeCamera.GetProjectMatrix();
            
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 3);
            }
        }

        public override void LoadContent(ContentManager content) {}
    }
}