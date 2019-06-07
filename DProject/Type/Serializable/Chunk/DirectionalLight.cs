using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class DirectionalLight
    {
        [Key(0)] public virtual bool Enabled { get; set; }
        
        [Key(1)] public virtual float DiffuseColorRed { get; set; }
        [Key(2)] public virtual float DiffuseColorGreen { get; set; }
        [Key(3)] public virtual float DiffuseColorBlue { get; set; }
        
        [Key(4)] public virtual float SpecularColorRed { get; set; }
        [Key(5)] public virtual float SpecularColorGreen { get; set; }
        [Key(6)] public virtual float SpecularColorBlue { get; set; }
        
        [Key(7)] public virtual float DirectionX { get; set; }
        [Key(8)] public virtual float DirectionY { get; set; }
        [Key(9)] public virtual float DirectionZ { get; set; }

        [IgnoreMember]
        public Vector3 DiffuseColor {
            get => new Vector3(DiffuseColorRed, DiffuseColorGreen, DiffuseColorBlue);
            set
            {
                DiffuseColorRed = value.X;
                DiffuseColorGreen = value.Y;
                DiffuseColorBlue = value.Z;
            }
        }
        
        [IgnoreMember]
        public Vector3 Direction {
            get => new Vector3(DirectionX, DirectionY, DirectionZ);
            set
            {
                DirectionX = value.X;
                DirectionY = value.Y;
                DirectionZ = value.Z;
            }
        }
        
        [IgnoreMember]
        public Vector3 SpecularColor {
            get => new Vector3(SpecularColorRed, SpecularColorGreen, SpecularColorBlue);
            set
            {
                SpecularColorRed = value.X;
                SpecularColorGreen = value.Y;
                SpecularColorBlue = value.Z;
            }
        }
    }
} 
