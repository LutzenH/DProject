using System;
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

        public enum Resolution
        {
            Horrible,
            Low,
            Medium,
            High,
            Full
        }

        private Resolution _resolution;

        public MapChunkTexture2D(GraphicsDevice graphicsDevice, ChunkData chunkData, int floor, Resolution resolution) : base(graphicsDevice, CalculateSize(resolution), CalculateSize(resolution))
        {            
            _xPos = chunkData.ChunkPositionX;
            _yPos = chunkData.ChunkPositionY;
            
            SetData(GenerateTileColors(chunkData.Tiles[floor], resolution));
        }

        private static int CalculateSize(Resolution resolution)
        {
            if (resolution == Resolution.Full)
                return ChunkLoaderEntity.ChunkSize * 2;
            else
                return ChunkLoaderEntity.ChunkSize / CalculateAdd(resolution);
        }

        private static int CalculateAdd(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Horrible:
                    return 8;
                case Resolution.Low:
                    return 4;
                case Resolution.Medium:
                    return 2;
                case Resolution.High:
                    return 1;
                case Resolution.Full:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private static Color[] GenerateTileColors(Tile[,] tilemap, Resolution resolution)
        {
            var size = CalculateSize(resolution);
            var add = CalculateAdd(resolution);
            var dataColors = new Color[size * size];
            
            if (resolution == Resolution.Full)
            {
                for (var x = 0; x < size; x++)
                {
                    for (var y = 0; y < size; y++)
                    {
                        if (x % 2 == 0)
                        {
                            if (y % 2 == 0)
                            {
                                dataColors[x + y * size] = Colors.ColorList[tilemap[x / 2, y / 2].ColorTopLeft].Color;
                            }
                            else
                            {
                                dataColors[x + y * size] = Colors.ColorList[tilemap[x / 2, y / 2].ColorBottomLeft].Color;
                            }
                        }
                        else
                        {
                            if (y % 2 == 0)
                            {
                                dataColors[x + y * size] = Colors.ColorList[tilemap[x / 2, y / 2].ColorTopRight].Color;
                            }
                            else
                            {
                                dataColors[x + y * size] = Colors.ColorList[tilemap[x / 2, y / 2].ColorBottomRight].Color;
                            }
                        }
                    }
                }
            }
            else
            {            
                for (var x = 0; x < size; x++)
                {
                    for (var y = 0; y < size; y++)
                    {
                        dataColors[x + y * size] = Colors.ColorList[tilemap[x * add, y * add].ColorTopLeft].Color;
                    }
                }   
            }
            
            return dataColors;
        }

        public int GetPositionX()
        {
            return _xPos;
        }

        public int GetPositionY()
        {
            return _yPos;
        }
    }
}