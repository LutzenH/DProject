using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type
{
    public class HeightMap
    {       
        private VertexPositionColor[] vertexPositions;

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
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            //Sends Vertex Information to the graphics-card
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), (width-1)*(height-1)*6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertexPositions);
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

        private static VertexPositionColor[] GenerateVertexPositions(float[,] heightmap)
        {
            int width = heightmap.GetLength(0) -1;
            int height = heightmap.GetLength(1) -1;

            VertexPositionColor[] vertexPositions = new VertexPositionColor[(width)*(height)*6];

            int vertexIndex = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x-0.5f, heightmap[x,y+1], y+0.5f), Color.Red);
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x-0.5f,  heightmap[x,y], y-0.5f), Color.Red);
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), Color.Red);
                    
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x-0.5f, heightmap[x,y], y-0.5f), Color.White);
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x+0.5f,  heightmap[x+1,y], y-0.5f), Color.White);
                    vertexPositions[vertexIndex++] = new VertexPositionColor(new Vector3 (x+0.5f, heightmap[x+1,y+1], y+0.5f), Color.White);
                }
            }
            
            return vertexPositions;
        }

        
    }
}