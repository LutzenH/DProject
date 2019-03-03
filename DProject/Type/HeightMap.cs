using System;
using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public class HeightMap
    {       
        private VertexPositionTextureColorNormal[] vertexPositions;

        private VertexBuffer vertexBuffer;
        private BasicEffect basicEffect;

        private int width;
        private int height;

        private GraphicsDevice GraphicsDevice;

        private float[,] heightmap;
        
        public HeightMap (int width, int height, float xOffset, float yOffset, float noiseScale)
        {
            this.width = width;
            this.height = height;

            heightmap = Noise.GenerateNoiseMap(width, height, xOffset, yOffset, noiseScale);
            vertexPositions = GenerateVertexPositions(heightmap);
        }

        public HeightMap(float[,] heightmap)
        {
            this.width = heightmap.GetLength(0)-1;
            this.height = heightmap.GetLength(1)-1;

            this.heightmap = heightmap;
            vertexPositions = GenerateVertexPositions(this.heightmap);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            
            //Graphics Effects
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.EnableDefaultLighting();
            basicEffect.VertexColorEnabled = true;

            basicEffect.TextureEnabled = true;
            
            //Sends Vertex Information to the graphics-card
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTextureColorNormal), (width)*(height)*6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTextureColorNormal>(vertexPositions);
        }

        public void Draw(Matrix projectMatrix, Matrix viewMatrix, Matrix worldMatrix, Texture2D texture2D)
        {
            basicEffect.Projection = projectMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            basicEffect.Texture = texture2D;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, (width)*(height)*3);
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

                    if (isAlternativeDiagonal(topLeft, topRight, bottomRight, bottomLeft))
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
            this.heightmap = heightmap;
            vertexPositions = GenerateVertexPositions(heightmap);
            vertexBuffer.SetData<VertexPositionTextureColorNormal>(vertexPositions);
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

        private static Boolean isAlternativeDiagonal(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {            
            if (Vector3.Distance(a, c) < Vector3.Distance(b, d))
            {
                return true;
            }
            else return false;
        }

        public float[,] GetHeightMap()
        {
            return heightmap;
        }
    }
}
