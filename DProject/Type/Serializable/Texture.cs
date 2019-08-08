using System;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Texture
    {
        [Key("location")]
        public virtual string TexturePath { get; set; }

        [Key("border_left")]
        public virtual double? BorderLeft { get; set; }
        
        [Key("border_right")]
        public virtual double? BorderRight { get; set; }
        
        [Key("border_top")]
        public virtual double? BorderTop { get; set; }
        
        [Key("border_bottom")]
        public virtual double? BorderBottom { get; set; }

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
        
        [IgnoreMember]
        public Rectangle TextureRectangle => new Rectangle(XOffset, YOffset, XSize, YSize);

        public Point GetTextureSize() => new Point(XSize, YSize);

        public Vector4 GetAdjustedTexturePosition()
        {
            return TexturePosition / TextureAtlasSize;
        }

        public Rectangle[] GetBorderSourceRectangles()
        {
            if (BorderLeft == null || BorderRight == null || BorderTop == null || BorderBottom == null)
                throw new Exception("One of the border settings has not been set for this texture.");
            
            var pixelBorderLeft = (int) Math.Round((double) (BorderLeft * XSize));
            var pixelBorderRight = (int) Math.Round((double) (BorderRight * XSize));
            var pixelBorderTop = (int) Math.Round((double) (BorderTop * YSize));
            var pixelBorderBottom = (int) Math.Round((double) (BorderBottom * YSize));
            
            return new[]
            {
                //Top Left
                new Rectangle(XOffset, YOffset, pixelBorderLeft, pixelBorderTop),
                
                //Top Middle
                new Rectangle(XOffset + pixelBorderLeft, YOffset, XSize - pixelBorderLeft - pixelBorderRight, pixelBorderTop),
                
                //Top Right
                new Rectangle(XOffset + XSize - pixelBorderRight, YOffset, pixelBorderRight, pixelBorderTop),
                
                //Middle Left
                new Rectangle(XOffset, YOffset + pixelBorderTop, pixelBorderLeft, YSize - pixelBorderTop - pixelBorderBottom), 
                
                //Middle
                new Rectangle(XOffset + pixelBorderLeft, YOffset + pixelBorderTop, XSize - pixelBorderLeft - pixelBorderRight, YSize - pixelBorderTop - pixelBorderBottom), 
                
                //Middle Right
                new Rectangle(XOffset + XSize - pixelBorderRight, YOffset + pixelBorderTop, pixelBorderRight, YSize - pixelBorderTop - pixelBorderBottom), 
                
                //Bottom Left
                new Rectangle(XOffset, YOffset + YSize - pixelBorderBottom, pixelBorderLeft, pixelBorderBottom), 
                
                //Bottom Middle
                new Rectangle(XOffset + pixelBorderLeft, YOffset + YSize - pixelBorderBottom, XSize - pixelBorderLeft - pixelBorderRight, pixelBorderBottom), 
                
                //Bottom Right
                new Rectangle(XOffset + XSize - pixelBorderRight, YOffset + YSize - pixelBorderBottom, pixelBorderRight, pixelBorderBottom)
            };
        }
    }
}
