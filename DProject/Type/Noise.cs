using LibNoise.Primitive;

namespace DProject.Type
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float noiseScale)
        {
            SimplexPerlin perlin = new SimplexPerlin();
            
            float[,] noiseMap = new float[mapWidth,mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {  
                    noiseMap[x,y] = perlin.GetValue(x/noiseScale, y/noiseScale);
                }
            }

            return noiseMap;
        }
    }
}