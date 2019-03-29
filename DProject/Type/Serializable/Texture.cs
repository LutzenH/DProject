using DProject.List;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Texture
    {
        [Key("name")]
        public string TextureName { get; set; }
        
        [Key("x_offset")]
        public int XOffset { get; set; }
        
        [Key("y_offset")]
        public int YOffset { get; set; }
        
        [Key("x_size")]
        public int XSize { get; set; }
        
        [Key("y_size")]
        public int YSize { get; set; }
           
        [IgnoreMember]
        public Vector4 TexturePosition => new Vector4(XOffset, YOffset, XOffset+XSize, YOffset+YSize);

        public Vector4 GetTexturePosition()
        {
            return TexturePosition;
        }

        public string GetTextureName()
        {
            return TextureName;
        }

        public Vector4 GetAdjustedTexturePosition()
        {
            return TexturePosition / Textures.TextureAtlasSize;
        }
    }
}