using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable.Chunk
{
    [MessagePackObject]
    public class Sky
    {
        [Key("name")]
        public virtual string Name { get; set; }
        
        [Key("fog")]
        public virtual Fog Fog { get; set; }
        
        [Key("background_color")]
        public virtual SerializableColor BackgroundColor { get; set; }
        
        [Key("ambient_light_color")]
        public virtual SerializableColor AmbientLightColor { get; set; }
        
        [Key("directional_light_0")]
        public virtual DirectionalLight DirectionalLight0 { get; set; }
        
        [Key("directional_light_1")]
        public virtual DirectionalLight DirectionalLight1 { get; set; }
        
        [Key("directional_light_2")]
        public virtual DirectionalLight DirectionalLight2 { get; set; }
    }
}
