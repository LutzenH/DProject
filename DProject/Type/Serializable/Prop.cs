using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Prop
    {
        [Key("name")]
        public virtual string Name { get; set; }

        [Key("path")]
        public virtual string AssetPath { get; set; }

        [Key("default_scale_x")]
        public virtual double ScaleX { get; set; }
        
        [Key("default_scale_y")]
        public virtual double ScaleY { get; set; }
        
        [Key("default_scale_z")]
        public virtual double ScaleZ { get; set; }
        
        [IgnoreMember]
        public Vector3 DefaultScale => new Vector3((float)ScaleX, (float)ScaleY, (float)ScaleZ);
    }
}
