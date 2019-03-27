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
        public virtual byte TopLeft { get; set; }
        [Key(1)]
        public virtual byte TopRight { get; set; }
        [Key(2)]
        public virtual byte BottomLeft { get; set; }
        [Key(3)]
        public virtual byte BottomRight { get; set; }
        
        //If the tile uses the other cross-diagonal triangles
        [Key(4)]
        public virtual bool IsAlternativeDiagonal { get; set; }
        
        //Texture of Tile
        [Key(5)]
        public virtual ushort? TileTextureIdTriangleOne { get; set; }
        [Key(6)]
        public virtual ushort? TileTextureIdTriangleTwo { get; set; }

        [Key(7)]
        public virtual ushort ColorTopLeft { get; set; }
        
        [Key(8)]
        public virtual ushort ColorTopRight { get; set; }
        
        [Key(9)]
        public virtual ushort ColorBottomLeft { get; set; }
        
        [Key(10)]
        public virtual ushort ColorBottomRight { get; set; }


        public void CalculateAlternativeDiagonal()
        {
            IsAlternativeDiagonal = HeightMap.IsAlternativeDiagonal(
                new Vector3(-0.5f, TopLeft, -0.5f),
                new Vector3(0.5f, TopRight, -0.5f),
                new Vector3(0.5f, BottomRight, 0.5f),
                new Vector3(-0.5f, BottomLeft, 0.5f)
            );
        }

        public void SetCornerHeight(byte height, TerrainEntity.TileCorner corner)
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