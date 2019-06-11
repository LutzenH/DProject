using MessagePack;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class Vertex
    {
        [Key(0)]
        public virtual ushort Height { get; set; }
        
        [Key(1)]
        public virtual ushort? TextureId { get; set; }

        [Key(2)]
        public virtual ushort ColorId { get; set; }
    }
}