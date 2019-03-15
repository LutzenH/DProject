using System;
using System.Runtime.Serialization;

namespace DProject.Type.Serializable
{
    [Serializable()]
    public class ChunkData : ISerializable
    {
        public int ChunkPositionX { get; set; }
        public int ChunkPositionY { get; set; }
        
        public Tile[][,] Tiles { get; set; }

        public ChunkData(int chunkPositionX, int chunkPositionY, Tile[][,] tiles)
        {
            ChunkPositionX = chunkPositionX;
            ChunkPositionY = chunkPositionY;
            
            Tiles = tiles;
        }

        public ChunkData(SerializationInfo info, StreamingContext context)
        {
            ChunkPositionX = (int) info.GetValue("x", typeof(int));
            ChunkPositionY = (int) info.GetValue("y", typeof(int));
            
            Tiles = (Tile[][,]) info.GetValue("tiles", typeof(Tile[][,]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", ChunkPositionX);
            info.AddValue("y", ChunkPositionY);
            info.AddValue("tiles", Tiles);
        }
    }
}