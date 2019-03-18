
using MessagePack;

namespace DProject.Type.Serializable
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
        public virtual Object[][] Objects { get; set; }
    }
}