using System.Collections.Generic;
using DProject.Type.Enum;
using MessagePack;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class ChunkData
    {
        [Key(0)]
        public virtual int ChunkPositionX { get; set; }
        [Key(1)]
        public virtual int ChunkPositionY { get; set; }
        [Key(2)]
        public virtual Tile[][,] Tiles { get; set; }

        [Key(3)]
        public virtual List<Object>[] Objects { get; set; }

        [Key(4)]
        public virtual LightingInfo LightingInfo { get; set; }

        [IgnoreMember]
        public ChunkStatus ChunkStatus { get; set; }
        
        public int GetFloorCount()
        {
            return Tiles.Length;
        }
    }
}