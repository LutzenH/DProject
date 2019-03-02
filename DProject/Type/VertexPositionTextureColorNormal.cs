using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public struct VertexPositionTextureColorNormal : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
 
        public VertexPositionTextureColorNormal(Vector3 position, Vector3 normal, Color Color, Vector2 textureCoordinate)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = Color;
            this.TextureCoordinate = textureCoordinate;
        }
        
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexPositionTextureColorNormal.VertexDeclaration;
            }
        }
    }
}