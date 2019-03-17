using System.IO;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Type.Rendering;
using DProject.Type.Serializable;
using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using Texture = DProject.Type.Texture;

namespace DProject.Entity.Chunk
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize
    {
        private HeightMap[] _heightMaps;
        private Texture2D _terrainTexture;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private GraphicsDevice _graphicsDevice;
        
        private readonly ChunkData _chunkData;

        private readonly BoundingSphere _boundingSphere;
        
        public ChunkStatus ChunkStatus { get; set; }

        public enum TileCorner { TopLeft, TopRight, BottomLeft, BottomRight }

        private const int DefaultFloorCount = 2;
        
        public TerrainEntity(int x, int y) : base(new Vector3(x*ChunkLoaderEntity.ChunkSize, 0, y*ChunkLoaderEntity.ChunkSize), Quaternion.Identity, new Vector3(1,1,1))
        {
            string path = "Content/Chunks/chunk_" + x + "_" + y + ".dat";

            if (File.Exists(path))
            {
                Stream stream = File.Open("Content/Chunks/chunk_" + x + "_" + y + ".dat", FileMode.Open);
                var bytes = stream;
                
                _chunkData = MessagePackSerializer.Deserialize<ChunkData>(bytes);
                stream.Close();

                ChunkStatus = ChunkStatus.Current;
            }
            else
            {                   
                float[,] heightmap = Noise.GenerateNoiseMap(ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, x*ChunkLoaderEntity.ChunkSize, y*ChunkLoaderEntity.ChunkSize, 50f);
                
                Tile[][,] tiles = new Tile[DefaultFloorCount][,];
                
                tiles[0] = HeightMap.GenerateTileMap(heightmap);
                tiles[1] = HeightMap.GenerateTileMap(ChunkLoaderEntity.ChunkSize, 1);
            
                _chunkData = new ChunkData()
                {
                    ChunkPositionX = x,
                    ChunkPositionY = y,
                    Tiles = tiles
                };

                ChunkStatus = ChunkStatus.Unserialized;

                //un-comment these to enable chunk creation to harddrive.
                //Stream stream = File.Open("Content/Chunks/chunk_" + x + "_" + y + ".dat", FileMode.Create);
                //var bytes = MessagePackSerializer.Serialize(_chunkData);
                //stream.Write(bytes, 0, bytes.Length);
            }

            _chunkPositionX = x;
            _chunkPositionY = y;

            _heightMaps = new HeightMap[DefaultFloorCount];
            
            for (var floor = 0; floor < _heightMaps.Length; floor++)
                _heightMaps[floor] = new HeightMap(_chunkData.Tiles[floor]);
            
            _boundingSphere = new BoundingSphere(new Vector3(x + ChunkLoaderEntity.ChunkSize/2, 0, y + ChunkLoaderEntity.ChunkSize/2), ChunkLoaderEntity.ChunkSize/1.6f);
        }

        public override void LoadContent(ContentManager content)
        {
            _terrainTexture = content.Load<Texture2D>(Textures.TextureAtlasLocation);
        }

        public void Draw(CameraEntity activeCamera)
        {
            if (activeCamera.GetBoundingFrustum().Intersects(_boundingSphere.Transform(GetWorldMatrix())))
            {
                foreach (var heightMap in _heightMaps)
                {
                    heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), _terrainTexture);
                }
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            foreach (var heightMap in _heightMaps)
            {
                heightMap.Initialize(graphicsDevice);
            }
        }

        public int GetChunkX()
        {
            return _chunkPositionX;
        }

        public int GetChunkY()
        {
            return _chunkPositionY;
        }

        public HeightMap GetHeightMap(int floor)
        {
            return _heightMaps[floor];
        }

        public void ChangeTileTexture(ushort textureId, int x, int y, int floor)
        {
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleOne = textureId;
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleTwo = textureId;

            _heightMaps[floor].SetHasUpdated(true);
            ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeTriangleTexture(ushort textureId, int x, int y, int floor, bool alternativeTriangle)
        {
            if(alternativeTriangle)
                _chunkData.Tiles[floor][x,y].TileTextureIdTriangleOne = textureId;
            else
                _chunkData.Tiles[floor][x,y].TileTextureIdTriangleTwo = textureId;

            _heightMaps[floor].SetHasUpdated(true);
            ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeColor(Color color, int x, int y, int floor)
        {
            _chunkData.Tiles[floor][x,y].ColorR = color.R;
            _chunkData.Tiles[floor][x,y].ColorG = color.G;
            _chunkData.Tiles[floor][x,y].ColorB = color.B;

            _heightMaps[floor].SetHasUpdated(true);
            ChunkStatus = ChunkStatus.Changed;
        }

        public void ChangeVertexHeight(float height, int x, int y, int floor, TileCorner corner)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                {
                    _chunkData.Tiles[floor][x, y].SetCornerHeight(height, TileCorner.TopLeft);
            
                    if(x > 0)
                        _chunkData.Tiles[floor][x-1,y].SetCornerHeight(height, TileCorner.TopRight);
            
                    if(y > 0)
                        _chunkData.Tiles[floor][x,y-1].SetCornerHeight(height, TileCorner.BottomLeft);
            
                    if(x > 0 && y > 0)
                        _chunkData.Tiles[floor][x-1,y-1].SetCornerHeight(height, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.TopRight:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(y > 0)
                        _chunkData.Tiles[floor][x,y-1].SetCornerHeight(height, TileCorner.BottomRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y > 0)
                        _chunkData.Tiles[floor][x+1,y-1].SetCornerHeight(height, TileCorner.BottomLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y].SetCornerHeight(height, TileCorner.TopLeft);
                    break;
                }
                
                case TileCorner.BottomLeft:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerHeight(height, TileCorner.BottomLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x,y+1].SetCornerHeight(height, TileCorner.TopLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1 && x > 0)
                        _chunkData.Tiles[floor][x-1,y+1].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(x > 0)
                        _chunkData.Tiles[floor][x-1,y].SetCornerHeight(height, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.BottomRight:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerHeight(height, TileCorner.BottomRight);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x,y+1].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y+1].SetCornerHeight(height, TileCorner.TopLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y].SetCornerHeight(height, TileCorner.BottomLeft);
                    break;
                }
            }

            _heightMaps[floor].SetHasUpdated(true);
            ChunkStatus = ChunkStatus.Changed;
        }

        public void UpdateHeightMap()
        {
            for (int floor = 0; floor < _heightMaps.Length; floor++)
            {
                if (_heightMaps[floor].GetHasUpdated())
                {
                    _heightMaps[floor] = new HeightMap(_chunkData.Tiles[floor]);
                    _heightMaps[floor].Initialize(_graphicsDevice);
                    _heightMaps[floor].SetHasUpdated(false);
                }
            }
        }

        public float GetTileHeight(int x, int y, int floor)
        {
            return (_chunkData.Tiles[floor][x, y].TopLeft
            +_chunkData.Tiles[floor][x, y].TopRight
            +_chunkData.Tiles[floor][x, y].BottomLeft
            +_chunkData.Tiles[floor][x, y].BottomRight
                   )/4;
        }

        public float GetVertexHeight(int x, int y, int floor, TileCorner corner)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                    return _chunkData.Tiles[floor][x, y].TopLeft;
                case TileCorner.TopRight:
                    return _chunkData.Tiles[floor][x, y].TopRight;
                case TileCorner.BottomLeft:
                    return _chunkData.Tiles[floor][x, y].BottomLeft;
                case TileCorner.BottomRight:
                    return _chunkData.Tiles[floor][x, y].BottomRight;
                default:
                    return 0f;
            }
        }
    }
}