using System;
using System.Runtime.Serialization;
using DProject.Entity.Chunk;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
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
        public string TileTextureNameTriangleOne { get; set; }
        public string TileTextureNameTriangleTwo { get; set; }
        
        //Tile Color
        public Color Color { get; set; }

        public Tile(float topLeft, float topRight, float bottomLeft, float bottomRight, bool isAlternativeDiagonal, string tileTextureNameTriangleOne, string tileTextureNameTriangleTwo, Color color)
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
            Color = color;
        }

        public Tile(SerializationInfo info, StreamingContext context)
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
        }

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