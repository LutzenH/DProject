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
        
        public void SetColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public string GetColorName()
        {
            return Name;
        }
    }
}