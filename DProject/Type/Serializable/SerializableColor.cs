using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class SerializableColor
    {
        [Key("name")]
        public virtual string Name { get; set; }
        [Key("red")]
        public virtual int Red { get; set; }
        [Key("green")]
        public virtual int Green { get; set; }
        [Key("blue")]
        public virtual int Blue { get; set; }
            
        [IgnoreMember]
        public Color Color => new Color(Red, Green, Blue);
        
        public string GetColorName()
        {
            return Name;
        }
    }
}