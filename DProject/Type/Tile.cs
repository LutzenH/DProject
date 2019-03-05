using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DProject.Type
{
    [Serializable()]
    public class Tile : ISerializable
    {
        //Vertex Heights
        public float TopLeft { get; set; }
        public float TopRight { get; set; }
        public float BottomLeft { get; set; }
        public float BottomRight { get; set; }
        
        //If the tile uses the other cross-diagonal triangles
        public bool IsAlternativeDiagonal { get; set; }
        
        //Texture of Tile
        public string TileTextureName { get; set; }
        
        //Tile Color
        public Color Color { get; set; }

        public Tile(float topLeft, float topRight, float bottomLeft, float bottomRight, bool isAlternativeDiagonal, string tileTextureName, Color color)
        {             
            //Vertex Heights
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            
            //If the tile uses the other cross-diagonal triangles
            IsAlternativeDiagonal = isAlternativeDiagonal;
            
            //Texture of Tile
            TileTextureName = tileTextureName;
            
            //Tile Color
            Color = color;
        }

        public Tile(SerializationInfo info, StreamingContext context)
        {
            TopLeft = (float) info.GetValue("TopLeft", typeof(float));
            TopRight = (float) info.GetValue("TopRight", typeof(float));
            BottomLeft = (float) info.GetValue("BottomLeft", typeof(float));
            BottomRight = (float) info.GetValue("BottomRight", typeof(float));
            
            IsAlternativeDiagonal = info.GetBoolean("IsAlternativeDiagonal");
            
            TileTextureName = info.GetString("TileTextureName");
            
            Color = new Color(info.GetByte("ColorR"), info.GetByte("ColorG"),info.GetByte("ColorB"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TopLeft", TopLeft);
            info.AddValue("TopRight", TopRight);
            info.AddValue("BottomLeft", BottomLeft);
            info.AddValue("BottomRight", BottomRight);
            
            info.AddValue("IsAlternativeDiagonal", IsAlternativeDiagonal);
            
            info.AddValue("TileTextureName", TileTextureName);
            
            info.AddValue("ColorR", Color.R);
            info.AddValue("ColorG", Color.G);
            info.AddValue("ColorB", Color.B);
        }
    }
}