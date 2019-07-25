using DProject.List;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Texture
    {
        [Key("location")]
        public string TexturePath { get; set; }

        [IgnoreMember]
        public int TextureAtlasSize { get; set; }

        [IgnoreMember]
        public int XOffset { get; set; }
        
        [IgnoreMember]
        public int YOffset { get; set; }
        
        [IgnoreMember]
        public int XSize { get; set; }
        
        [IgnoreMember]
        public int YSize { get; set; }
           
        [IgnoreMember]
        public Vector4 TexturePosition => new Vector4(XOffset, YOffset, XOffset+XSize, YOffset+YSize);

        public Vector4 GetTexturePosition()
        {
            return TexturePosition;
        }

        public Vector4 GetAdjustedTexturePosition()
        {
            return TexturePosition / TextureAtlasSize;
        }
    }
}