using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class Fog
    {
        [Key(0)] public virtual bool Enabled { get; set; }
        
        [Key(1)] public virtual float FogStart { get; set; }
        [Key(2)] public virtual float FogEnd { get; set; }
        
        [Key(3)] public virtual float ColorRed { get; set; }
        [Key(4)] public virtual float ColorGreen { get; set; }
        [Key(5)] public virtual float ColorBlue { get; set; }

        [IgnoreMember]
        public Vector3 Color {
            get => new Vector3(ColorRed, ColorGreen, ColorBlue);
            set
            {
                ColorRed = value.X;
                ColorGreen = value.Y;
                ColorBlue = value.Z;
            }
        }
    }
} 
