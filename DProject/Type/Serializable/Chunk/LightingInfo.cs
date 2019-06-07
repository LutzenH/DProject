using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class LightingInfo
    {
        [Key(0)] public virtual Fog Fog { get; set; }
        [Key(1)] public virtual SerializableColor BackgroundColor { get; set; }
        [Key(2)] public virtual SerializableColor AmbientLightColor { get; set; }
        [Key(3)] public virtual DirectionalLight DirectionalLight0 { get; set; }
        [Key(4)] public virtual DirectionalLight DirectionalLight1 { get; set; }
        [Key(5)] public virtual DirectionalLight DirectionalLight2 { get; set; }
    }
}
