using DProject.Type.Rendering;
using Microsoft.Xna.Framework;

namespace DProject.Game.Component
{
    public class WaterPlaneComponent
    {
        private Vector2 _size;
        public VertexPositionTextureColorNormal[] VertexList { get; private set; }

        public WaterPlaneComponent()
        {
            Size = new Vector2(64, 64);
            VertexList = CreateWaterPlane(Size);
        }
        
        public Vector2 Size { get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    VertexList = CreateWaterPlane(_size);
                }
            }
        }

        private static VertexPositionTextureColorNormal[] CreateWaterPlane(Vector2 size)
        {
            return new[]
            {
                new VertexPositionTextureColorNormal(new Vector3(0, 0, size.Y), Vector3.Up, Color.White, Vector2.UnitY), 
                new VertexPositionTextureColorNormal(Vector3.Zero, Vector3.Up, Color.White, Vector2.Zero),
                new VertexPositionTextureColorNormal(new Vector3(size.X, 0, size.Y), Vector3.Up, Color.White, Vector2.One),
                new VertexPositionTextureColorNormal(Vector3.Zero, Vector3.Up, Color.White, Vector2.Zero),
                new VertexPositionTextureColorNormal(new Vector3(size.X, 0, 0), Vector3.Up, Color.White, Vector2.UnitX), 
                new VertexPositionTextureColorNormal(new Vector3(size.X, 0, size.Y), Vector3.Up, Color.White, Vector2.One)
            };
        }
    }
}
