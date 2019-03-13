using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity.Debug
{
    public class CornerIndicatorEntity : AbstractEntity, IInitialize, IDrawable
    {
        private BasicEffect _basicEffect;

        private readonly Vector3 _lineStartPoint;
        private readonly Vector3 _lineEndPointX;
        private readonly Vector3 _lineEndPoint;

        private readonly Color _color;

        private VertexBuffer _vertexBuffer;

        private GraphicsDevice _graphicsDevice;

        public CornerIndicatorEntity(Vector3 position, TerrainEntity.TileCorner tileCorner, Color color) : base(position, GenerateRotationFromCorner(tileCorner), new Vector3(1, 1, 1))
        {
            _lineStartPoint = new Vector3(-0.5f, 0.1f, -0.5f);
            _lineEndPointX = new Vector3(-0.5f, 0.1f, 0f);
            _lineEndPoint = new Vector3(0f, 0.1f, -0.5f);
                        
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
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(
                new[]
                {
                    new VertexPositionColor(_lineStartPoint, _color), new VertexPositionColor(_lineEndPointX, _color),
                    new VertexPositionColor(_lineEndPointX, _color), new VertexPositionColor(_lineEndPoint, _color),
                    new VertexPositionColor(_lineEndPoint, _color), new VertexPositionColor(_lineStartPoint, _color)
                });
        }


        public void Draw(CameraEntity activeCamera)
        {
            _basicEffect.View = activeCamera.GetViewMatrix();
            _basicEffect.World = GetWorldMatrix();
            _basicEffect.Projection = activeCamera.GetProjectMatrix();

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 3);
            }
        }

        private static Quaternion GenerateRotationFromCorner(TerrainEntity.TileCorner corner)
        {   
            switch (corner)
            {
                case TerrainEntity.TileCorner.TopLeft:
                    return Quaternion.CreateFromYawPitchRoll(0f,0f,0f);
                case TerrainEntity.TileCorner.TopRight:
                    return Quaternion.CreateFromYawPitchRoll(4.68f,0f,0f);
                    break;
                case TerrainEntity.TileCorner.BottomLeft:
                    return Quaternion.CreateFromYawPitchRoll(1.56f,0,0);
                    break;
                case TerrainEntity.TileCorner.BottomRight:
                    return Quaternion.CreateFromYawPitchRoll(3.12f,0f,0f);
            }
            
            return Quaternion.Identity;
        }

        public void SetRotation(TerrainEntity.TileCorner corner)
        {
            SetRotation(GenerateRotationFromCorner(corner));
        }

        public override void LoadContent(ContentManager content) {}
    }
}