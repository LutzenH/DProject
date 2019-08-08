using System;
using DProject.Entity.Chunk;
using DProject.List;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Map
{
    public class MapChunkTexture2D : Texture2D
    {
        private readonly (int, int) _position;

        public enum Resolution
        {
            Horrible,
            Low,
            Medium,
            High,
            Full
        }
        
        public MapChunkTexture2D(GraphicsDevice graphicsDevice, ChunkData chunkData, Resolution resolution) : base(graphicsDevice, CalculateSize(resolution), CalculateSize(resolution))
        {            
            _position = (chunkData.ChunkPositionX, chunkData.ChunkPositionY);
            
            SetData(GenerateTileColors(chunkData.VertexMap, resolution));
        }

        private static int CalculateSize(Resolution resolution)
        {
            return ChunkLoaderEntity.ChunkSize / CalculateAdd(resolution);
        }

        private static int CalculateAdd(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Horrible:
                    return 16;
                case Resolution.Low:
                    return 8;
                case Resolution.Medium:
                    return 4;
                case Resolution.High:
                    return 2;
                case Resolution.Full:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private static Color[] GenerateTileColors(Vertex[,] tilemap, Resolution resolution)
        {
            var size = CalculateSize(resolution);
            var add = CalculateAdd(resolution);
            var dataColors = new Color[size * size];

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    dataColors[x + y * size] = Colors.ColorList[tilemap[x * add, y * add].ColorId].Color;
                }
            }

            return dataColors;
        }

        public int GetPositionX()
        {
            return _position.Item1;
        }

        public int GetPositionY()
        {
            return _position.Item2;
        }

        public (int, int) GetPosition()
        {
            return _position;
        }
    }
}