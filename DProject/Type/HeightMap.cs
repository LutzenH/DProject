using System;
using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public class HeightMap
    {       
        private VertexPositionNormalTexture[] vertexPositions;

        private VertexBuffer vertexBuffer;
        private BasicEffect basicEffect;

        private int width;
        private int height;

        private GraphicsDevice GraphicsDevice;
        
        public HeightMap (int width, int height, float noiseScale)
        {
            this.width = width;
            this.height = height;
            
            vertexPositions = GenerateVertexPositions(Noise.GenerateNoiseMap(width, height, noiseScale));
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            
            //Graphics Effects
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.EnableDefaultLighting();
            
            //Sends Vertex Information to the graphics-card
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), (width-1)*(height-1)*6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertexPositions);
        }

        public void Draw(Matrix projectMatrix, Matrix viewMatrix, Matrix worldMatrix)
        {
            basicEffect.Projection = projectMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, (width-1)*(height-1)*6/3);
            }
        }

        private static VertexPositionNormalTexture[] GenerateVertexPositions(float[,] heightmap)
        {
            int width = heightmap.GetLength(0) -1;
            int height = heightmap.GetLength(1) -1;

            VertexPositionNormalTexture[] vertexPositions = new VertexPositionNormalTexture[(width)*(height)*6];

            int vertexIndex = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 normal = GenerateNormalDirection(
                        new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f),
                        new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f),
                        new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f)
                        );
                    
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f), normal, Vector2.Zero);
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f), normal, Vector2.Zero);
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), normal, Vector2.Zero);
                    
                    normal = GenerateNormalDirection(
                        new Vector3 (x-0.5f, heightmap[x,y], y-0.5f),
                        new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f),
                        new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f)
                    );
                    
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x-0.5f, heightmap[x,y], y-0.5f), normal, Vector2.Zero);
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f), normal, Vector2.Zero);
                    vertexPositions[vertexIndex++] = new VertexPositionNormalTexture(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), normal, Vector2.Zero);
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
    }
}