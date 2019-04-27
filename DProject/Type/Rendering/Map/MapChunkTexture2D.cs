using DProject.Entity.Chunk;
using DProject.List;
using DProject.Type.Serializable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Map
{
    public class MapChunkTexture2D : Texture2D
    {
        private readonly int _xPos;
        private readonly int _yPos;
                
        public MapChunkTexture2D(GraphicsDevice graphicsDevice, ChunkData chunkData, int floor) : base(graphicsDevice, ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize)
        {
            _xPos = chunkData.ChunkPositionX;
            _yPos = chunkData.ChunkPositionY;
            
            SetData(GenerateTileColors(chunkData.Tiles[floor]));
        }

        private static Color[] GenerateTileColors(Tile[,] tilemap)
        {
            var dataColors = new Color[ChunkLoaderEntity.ChunkSize * ChunkLoaderEntity.ChunkSize];
            
            for (var x = 0; x < ChunkLoaderEntity.ChunkSize; x++)
            {
                for (var y = 0; y < ChunkLoaderEntity.ChunkSize; y++)
                {
                    dataColors[x + y * ChunkLoaderEntity.ChunkSize] = Colors.ColorList[tilemap[x,y].ColorTopLeft].Color;
                }
            }

            return dataColors;
        }

        private int GetPositionX()
        {
            return _xPos;
        }

        private int GetPositionY()
        {
            return _yPos;
        }
    }
}