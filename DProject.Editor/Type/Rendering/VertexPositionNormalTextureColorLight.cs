using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public struct VertexPositionNormalTextureColorLight : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Color Color;
        public Color LightingInfo;

        public VertexPositionNormalTextureColorLight(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Color color, Color lightingInfo)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Color = color;
            LightingInfo = lightingInfo;
        }
        
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(36, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}
