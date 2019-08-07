using MessagePack;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Font
    {
        [Key("location")]
        public virtual string Location { get; set; }

        [IgnoreMember]
        public virtual SpriteFont SpriteFont { get; set; }
    }
} 
