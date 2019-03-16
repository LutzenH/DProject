using DProject.Entity.Chunk;
using DProject.Type.Rendering;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Tile
    {
        //Vertex Heights
        [Key(0)]
        public virtual float TopLeft { get; set; }
        [Key(1)]
        public virtual float TopRight { get; set; }
        [Key(2)]
        public virtual float BottomLeft { get; set; }
        [Key(3)]
        public virtual float BottomRight { get; set; }
        
        //If the tile uses the other cross-diagonal triangles
        [Key(4)]
        public virtual bool IsAlternativeDiagonal { get; set; }
        
        //Texture of Tile
        [Key(5)]
        public virtual string TileTextureNameTriangleOne { get; set; }
        [Key(6)]
        public virtual string TileTextureNameTriangleTwo { get; set; }

        [Key(7)]
        public virtual byte ColorR { get; set; }
        
        [Key(8)]
        public virtual byte ColorG { get; set; }
        
        [Key(9)]
        public virtual byte ColorB { get; set; }

        //Tile Color
        [IgnoreMember]
        public Color Color => new Color(ColorR, ColorG, ColorB);

        /*public Tile(float topLeft, float topRight, float bottomLeft, float bottomRight, bool isAlternativeDiagonal, string tileTextureNameTriangleOne, string tileTextureNameTriangleTwo, Color color)
        {             
            //Vertex Heights
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            
            //If the tile uses the other cross-diagonal triangles
            IsAlternativeDiagonal = isAlternativeDiagonal;
            
            //Texture of Tile
            TileTextureNameTriangleOne = tileTextureNameTriangleOne;
            TileTextureNameTriangleTwo = tileTextureNameTriangleTwo;
            
            //Tile Color
            ColorR = color.R;
            ColorG = color.G;
            ColorB = ColorB;
        }*/

        /*public Tile(SerializationInfo info, StreamingContext context)
        {
            TopLeft = (float) info.GetValue("TopLeft", typeof(float));
            TopRight = (float) info.GetValue("TopRight", typeof(float));
            BottomLeft = (float) info.GetValue("BottomLeft", typeof(float));
            BottomRight = (float) info.GetValue("BottomRight", typeof(float));
            
            IsAlternativeDiagonal = info.GetBoolean("IsAlternativeDiagonal");
            
            TileTextureNameTriangleOne = info.GetString("TileTextureNameTriangleOne");
            TileTextureNameTriangleTwo = info.GetString("TileTextureNameTriangleTwo");
            
            Color = new Color(info.GetByte("ColorR"), info.GetByte("ColorG"),info.GetByte("ColorB"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TopLeft", TopLeft);
            info.AddValue("TopRight", TopRight);
            info.AddValue("BottomLeft", BottomLeft);
            info.AddValue("BottomRight", BottomRight);
            
            info.AddValue("IsAlternativeDiagonal", IsAlternativeDiagonal);
            
            info.AddValue("TileTextureNameTriangleOne", TileTextureNameTriangleOne);
            info.AddValue("TileTextureNameTriangleTwo", TileTextureNameTriangleTwo);
            
            info.AddValue("ColorR", Color.R);
            info.AddValue("ColorG", Color.G);
            info.AddValue("ColorB", Color.B);
        }*/

        public void CalculateAlternativeDiagonal()
        {
            IsAlternativeDiagonal = HeightMap.IsAlternativeDiagonal(
                new Vector3(-0.5f, TopLeft, -0.5f),
                new Vector3(0.5f, TopRight, -0.5f),
                new Vector3(0.5f, BottomRight, 0.5f),
                new Vector3(-0.5f, BottomLeft, 0.5f)
            );
        }

        public void SetCornerHeight(float height, TerrainEntity.TileCorner corner)
        {
            switch (corner)
            {
                case TerrainEntity.TileCorner.TopLeft:
                    TopLeft = height;
                    break;
                case TerrainEntity.TileCorner.TopRight:
                    TopRight = height;
                    break;
                case TerrainEntity.TileCorner.BottomLeft:
                    BottomLeft = height;
                    break;
                case TerrainEntity.TileCorner.BottomRight:
                    BottomRight = height;
                    break;
            }

            CalculateAlternativeDiagonal();
        }
    }
}