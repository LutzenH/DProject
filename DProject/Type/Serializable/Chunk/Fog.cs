using MessagePack;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class Fog
    {
        [Key("start")] 
        public virtual float FogStart { get; set; }
        
        [Key("end")]
        public virtual float FogEnd { get; set; }
        
        [Key("color")]
        public virtual SerializableColor Color { get; set; }
    }
} 
