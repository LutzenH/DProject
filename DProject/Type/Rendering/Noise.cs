using LibNoise.Primitive;

namespace DProject.Type.Rendering
{
    public static class Noise
    {
        public static ushort[,] GenerateNoiseMap(int mapWidth, int mapHeight, float xOffset, float yOffset, float noiseScale)
        {
            var perlin = new SimplexPerlin();
            
            var noiseMap = new ushort[mapWidth + 1,mapHeight + 1];

            for (var x = 0; x < mapWidth + 1; x++)
            {
                for (var y = 0; y < mapHeight + 1; y++)
                {  
                    noiseMap[x,y] = (ushort) (1024 + perlin.GetValue((x+xOffset)/noiseScale, (y+yOffset)/noiseScale) * 1024);
                }
            }

            return noiseMap;
        }
    }
}