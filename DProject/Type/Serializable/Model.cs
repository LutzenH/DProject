using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Model
    {
        [Key("name")]
        public virtual string Name { get; set; }

        [Key("path")]
        public virtual string AssetPath { get; set; }
    }
}
