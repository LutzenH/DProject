using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity.Debug
{
    public class LineFrameEntity : AbstractEntity, IInitialize, IDrawable
    {
        private BasicEffect _basicEffect;
        
        private readonly Vector3 _lineStartPoint;
        private readonly Vector3 _lineEndPointX;
        private readonly Vector3 _lineEndPointY;
        private readonly Vector3 _lineEndPoint;

        private readonly Color _color;
        
        private VertexBuffer _vertexBuffer;
        
        private GraphicsDevice _graphicsDevice;
        
        public LineFrameEntity(Vector3 position, int sizeX, int sizeY, Color color) : base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            float difference = 0.5f;
            
            _lineStartPoint = new Vector3(0 - difference/2,8f,0 - difference/2);
            _lineEndPointX = new Vector3(_lineStartPoint.X+sizeX-difference, _lineStartPoint.Y, _lineStartPoint.Z);
            _lineEndPointY = new Vector3(_lineStartPoint.X+sizeX-difference, _lineStartPoint.Y, _lineStartPoint.Z+sizeY-difference);
            _lineEndPoint = new Vector3(_lineStartPoint.X, _lineStartPoint.Y, _lineStartPoint.Z+sizeY-difference);

            _color = color;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.Alpha = 1.0f;
            _basicEffect.LightingEnabled = false;
            _basicEffect.VertexColorEnabled = true;
            
            //Sends Vertex Information to the graphics-card
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 8, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(
                new[]
                {
                    new VertexPositionColor(_lineStartPoint, _color),  new VertexPositionColor(_lineEndPointX, _color),
                    new VertexPositionColor(_lineEndPointX, _color),  new VertexPositionColor(_lineEndPointY, _color),
                    new VertexPositionColor(_lineEndPointY, _color),  new VertexPositionColor(_lineEndPoint, _color),
                    new VertexPositionColor(_lineEndPoint, _color),  new VertexPositionColor(_lineStartPoint, _color)
                });
        }


        public void Draw(CameraEntity activeCamera, ShaderManager shaderManager)
        {
            _basicEffect.View = activeCamera.GetViewMatrix();
            _basicEffect.World = GetWorldMatrix();
            _basicEffect.Projection = activeCamera.GetProjectMatrix();
            
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 4);
            }
        }
    }
}