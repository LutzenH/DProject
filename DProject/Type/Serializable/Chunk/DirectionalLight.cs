using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class DirectionalLight
    {
        [Key("diffuse_color")]
        public virtual SerializableColor DiffuseColor { get; set; }
        
        [Key("specular_color")]
        public virtual SerializableColor SpecularColor { get; set; }

        [Key("direction_x")] public virtual double DirectionX { get; set; }
        [Key("direction_y")] public virtual double DirectionY { get; set; }
        [Key("direction_z")] public virtual double DirectionZ { get; set; }

        [IgnoreMember]
        public Vector3 Direction {
            get => new Vector3((float) DirectionX, (float) DirectionY, (float) DirectionZ);
            set
            {
                DirectionX = value.X;
                DirectionY = value.Y;
                DirectionZ = value.Z;
            }
        }
    }
} 
