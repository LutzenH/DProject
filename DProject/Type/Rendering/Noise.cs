using LibNoise.Primitive;

namespace DProject.Type.Rendering
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float xOffset, float yOffset, float noiseScale)
        {
            SimplexPerlin perlin = new SimplexPerlin();
            
            float[,] noiseMap = new float[mapWidth+1,mapHeight+1];

            for (int x = 0; x < mapWidth+1; x++)
            {
                for (int y = 0; y < mapHeight+1; y++)
                {  
                    noiseMap[x,y] = perlin.GetValue((x+xOffset)/noiseScale, (y+yOffset)/noiseScale) * 5;
                }
            }

            return noiseMap;
        }
    }
}