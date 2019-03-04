using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public class HeightMap
    {       
        private VertexPositionTextureColorNormal[] _vertexPositions;

        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        private readonly int _width;
        private readonly int _height;

        private GraphicsDevice _graphicsDevice;

        private float[,] _heightmap;
        
        public HeightMap (int width, int height, float xOffset, float yOffset, float noiseScale)
        {
            _width = width;
            _height = height;

            _heightmap = Noise.GenerateNoiseMap(width, height, xOffset, yOffset, noiseScale);
            _vertexPositions = GenerateVertexPositions(_heightmap);
        }

        public HeightMap(float[,] heightmap)
        {
            _width = heightmap.GetLength(0)-1;
            _height = heightmap.GetLength(1)-1;

            _heightmap = heightmap;
            _vertexPositions = GenerateVertexPositions(_heightmap);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            //Graphics Effects
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.Alpha = 1.0f;
            _basicEffect.EnableDefaultLighting();
            _basicEffect.VertexColorEnabled = true;

            _basicEffect.TextureEnabled = true;
            
            //Sends Vertex Information to the graphics-card
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTextureColorNormal), (_width)*(_height)*6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(_vertexPositions);
        }

        public void Draw(Matrix projectMatrix, Matrix viewMatrix, Matrix worldMatrix, Texture2D texture2D)
        {
            _basicEffect.Projection = projectMatrix;
            _basicEffect.View = viewMatrix;
            _basicEffect.World = worldMatrix;

            _basicEffect.Texture = texture2D;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, (_width)*(_height)*3);
            }
        }

        private static VertexPositionTextureColorNormal[] GenerateVertexPositions(float[,] heightmap)
        {
            int width = heightmap.GetLength(0) -1;
            int height = heightmap.GetLength(1) -1;

            VertexPositionTextureColorNormal[] vertexPositions = new VertexPositionTextureColorNormal[(width)*(height)*6];

            int vertexIndex = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 topLeft = new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f);
                    Vector3 topRight = new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f);
                    Vector3 bottomLeft = new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f);
                    Vector3 bottomRight = new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f);
                    
                    //Color color = new Color(topLeft.Y/5 +1, topLeft.Y/5 + 1, topLeft.Y/5 + 1);
                    Color color = Color.White;
                    
                    Vector4 texturePosition = GetTextureCoordinate(topLeft);
                    Vector3 normal = GenerateNormalDirection(bottomLeft, topLeft, bottomRight);

                    if (IsAlternativeDiagonal(topLeft, topRight, bottomRight, bottomLeft))
                    {
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePosition.X,texturePosition.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePosition.X,texturePosition.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePosition.Z,texturePosition.Y));
                        
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePosition.X,texturePosition.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePosition.Z,texturePosition.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePosition.Z,texturePosition.Y));
                    }
                    else
                    {                        
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePosition.X,texturePosition.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePosition.X,texturePosition.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePosition.Z,texturePosition.W));
                        
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePosition.X,texturePosition.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePosition.Z,texturePosition.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePosition.Z,texturePosition.Y));
                    }
                }
            }
            
            return vertexPositions;
        }

        private static Vector3 GenerateNormalDirection(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 sum = Vector3.Zero;
            
            var n0 = (vertex2 - vertex1) * 10f;
            var n1 = (vertex3 - vertex2) * 10f;

            var cnorm = Vector3.Cross(n0, n1);
            
            sum += cnorm;

            return sum;
        }

        public void UpdateTerrain(float[,] heightmap)
        {
            _heightmap = heightmap;
            _vertexPositions = GenerateVertexPositions(heightmap);
            _vertexBuffer.SetData(_vertexPositions);
        }

        private static Vector4 GetTextureCoordinate(Vector3 topLeft)
        {            
            if (topLeft.Y < -1.2f)
                return Textures.TextureList["savanna_earth"].GetAdjustedTexturePosition();
            else if (topLeft.Y < 0f)
                return Textures.TextureList["savanna_grass_dry"].GetAdjustedTexturePosition();
            else
                return Textures.TextureList["savanna_grass"].GetAdjustedTexturePosition();
        }

        private static bool IsAlternativeDiagonal(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Distance(a, c) < Vector3.Distance(b, d);
        }

        public float[,] GetHeightMap()
        {
            return _heightmap;
        }
    }
}
