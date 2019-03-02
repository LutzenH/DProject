using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public class HeightMap
    {       
        private VertexPositionTextureColorNormal[] vertexPositions;
        private float[,] heightdata;

        private VertexBuffer vertexBuffer;
        private BasicEffect basicEffect;

        private int width;
        private int height;

        private float xOffset;
        private float yOffset;

        private GraphicsDevice GraphicsDevice;
        
        public HeightMap (int width, int height, float xOffset, float yOffset, float noiseScale)
        {
            this.width = width;
            this.height = height;

            this.xOffset = xOffset;
            this.yOffset = yOffset;

            this.heightdata = Noise.GenerateNoiseMap(width, height, xOffset, yOffset, noiseScale);
            
            vertexPositions = GenerateVertexPositions(heightdata);
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
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, (width)*(height)*6/3);
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
                    float floatColor = (heightmap[x, y] + 1)/2;
                    Color color = new Color(floatColor, floatColor, floatColor);
                    
                    Vector3 normal = GenerateNormalDirection(
                        new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f),
                        new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f),
                        new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f)
                        );
                    
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f), normal, color, new Vector2(0,0));
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f), normal, color, new Vector2(0,1));
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), normal, color, new Vector2(1,0));
                    
                    normal = GenerateNormalDirection(
                        new Vector3 (x-0.5f, heightmap[x,y], y-0.5f),
                        new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f),
                        new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f)
                    );
                    
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x-0.5f, heightmap[x,y], y-0.5f), normal, color, new Vector2(0,1));
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f), normal, color, new Vector2(1,1));
                    vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), normal, color, new Vector2(1,0));
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

        public void UpdateTerrain(float noiseScale)
        {
            vertexPositions = GenerateVertexPositions(Noise.GenerateNoiseMap(width, height, xOffset, yOffset, noiseScale));
            vertexBuffer.SetData<VertexPositionTextureColorNormal>(vertexPositions);
        }
        
        public float[,] GetHeightData() {
            return heightdata;
        }
    }
}
