using DProject.List;
using DProject.Type.Serializable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class HeightMap
    {       
        private VertexPositionTextureColorNormal[] _vertexPositions;

        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        private readonly int _width;
        private readonly int _height;

        private GraphicsDevice _graphicsDevice;

        public HeightMap(ChunkData chunkData)
        {
            _width = chunkData.Tiles.GetLength(0);
            _height = chunkData.Tiles.GetLength(1);

            _vertexPositions = GenerateVertexPositions(chunkData.Tiles);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            //Graphics Effects
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.Alpha = 1.0f;
            
            _basicEffect.LightingEnabled = true;
            _basicEffect.AmbientLightColor = new Vector3(0.15f,0.15f,0.15f);
            _basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
            _basicEffect.DirectionalLight0.Direction = new Vector3(1, 0.5f, 0);

            _basicEffect.VertexColorEnabled = true;

            _basicEffect.FogEnabled = true;
            _basicEffect.FogColor = Color.DarkGray.ToVector3();
            _basicEffect.FogStart = 100f;
            _basicEffect.FogEnd = 200f;

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

        private static VertexPositionTextureColorNormal[] GenerateVertexPositions(Tile[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            VertexPositionTextureColorNormal[] vertexPositions = new VertexPositionTextureColorNormal[(width)*(height)*6];

            int vertexIndex = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 topLeft = new Vector3 (x-0.5f,  tiles[x,y].TopLeft, y-0.5f);
                    Vector3 topRight = new Vector3 (x+0.5f,  tiles[x,y].TopRight, y-0.5f);
                    Vector3 bottomLeft = new Vector3 (x-0.5f, tiles[x,y].BottomLeft, y+0.5f);
                    Vector3 bottomRight = new Vector3 (x+0.5f, tiles[x,y].BottomRight, y+0.5f);

                    Color color = tiles[x, y].Color;
                    
                    Vector4 texturePositionTexture0 = Textures.TextureList[tiles[x,y].TileTextureNameTriangleOne].GetAdjustedTexturePosition();
                    Vector4 texturePositionTexture1 = Textures.TextureList[tiles[x,y].TileTextureNameTriangleTwo].GetAdjustedTexturePosition();
                    
                    Vector3 normal;

                    if (tiles[x,y].IsAlternativeDiagonal)
                    {
                        normal = GenerateNormalDirection(bottomLeft, topLeft, bottomRight);
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePositionTexture0.X,texturePositionTexture0.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePositionTexture0.X,texturePositionTexture0.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.Y));
                           
                        normal = GenerateNormalDirection(topLeft, topRight, bottomRight);
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePositionTexture1.X,texturePositionTexture1.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.Y));
                    }
                    else
                    {     
                        normal = GenerateNormalDirection(bottomLeft, topLeft, topRight);
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePositionTexture1.X,texturePositionTexture1.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, color, new Vector2(texturePositionTexture1.X,texturePositionTexture1.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.W));
                            
                        normal = GenerateNormalDirection(topRight, bottomRight, bottomLeft);
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, color, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.W));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, color, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.Y));
                        vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, color, new Vector2(texturePositionTexture0.X,texturePositionTexture0.Y));
                    }
                }
            }

            return vertexPositions;
        }

        public static Tile[,] GenerateTileMap(float[,] heightmap)
        {
            int width = heightmap.GetLength(0)-1;
            int height = heightmap.GetLength(1)-1;

            Tile[,] tempTileMap = new Tile[width,height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float topLeft = heightmap[x,y];
                    float topRight = heightmap[x+1,y];
                    float bottomLeft = heightmap[x,y+1];
                    float bottomRight = heightmap[x+1,y+1];
                    
                    bool isAlternativeDiagonal = IsAlternativeDiagonal(
                        new Vector3(x, topLeft, y),
                        new Vector3(x+1, topRight, y),
                        new Vector3(x+1, bottomRight, y+1),
                        new Vector3(x, bottomLeft, y+1)
                        );
                    
                    string tileTextureName;
                    
                    if (topLeft < -1.2f)
                        tileTextureName = "savanna_earth";
                    else if (topLeft < 0f)
                        tileTextureName = "savanna_grass_dry";
                    else
                        tileTextureName = "savanna_grass";
                    
                    Color color = Color.White;
                    
                    tempTileMap[x,y] = new Tile(topLeft, topRight, bottomLeft, bottomRight, isAlternativeDiagonal, tileTextureName, tileTextureName, color);
                }
            }

            return tempTileMap;
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

        public static bool IsAlternativeDiagonal(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Distance(a, c) < Vector3.Distance(b, d);
        }
    }
}
