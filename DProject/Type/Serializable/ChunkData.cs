
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

        /*public ChunkData(int chunkPositionX, int chunkPositionY, Tile[][,] tiles)
        {
            ChunkPositionX = chunkPositionX;
            ChunkPositionY = chunkPositionY;
            
            Tiles = tiles;
        }*/

        /*public ChunkData(SerializationInfo info, StreamingContext context)
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
        }*/
    }
}