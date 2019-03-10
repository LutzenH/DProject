using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Type.Rendering;
using DProject.Type.Serializable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity.Chunk
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize
    {
        private HeightMap _heightMap;
        private Texture2D _terrainTexture;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private GraphicsDevice _graphicsDevice;
        
        private readonly ChunkData _chunkData;

        private readonly BoundingSphere _boundingSphere;

        private bool _updatedHeightmap;
        
        public ChunkStatus ChunkStatus { get; set; }

        public enum TileCorner { TopLeft, TopRight, BottomLeft, BottomRight }

        public TerrainEntity(int x, int y) : base(new Vector3(x*ChunkLoaderEntity.ChunkSize, 0, y*ChunkLoaderEntity.ChunkSize), Quaternion.Identity, new Vector3(1,1,1))
        {
            BinaryFormatter bf = new BinaryFormatter();
            string path = "Content/Chunks/chunk_" + x + "_" + y + ".dat";

            if (File.Exists(path))
            {
                Stream stream = File.Open("Content/Chunks/chunk_" + x + "_" + y + ".dat", FileMode.Open);
                
                _chunkData = (ChunkData) bf.Deserialize(stream);
                stream.Close();

                ChunkStatus = ChunkStatus.Current;
            }
            else
            {                   
                float[,] heightmap = Noise.GenerateNoiseMap(ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, x*ChunkLoaderEntity.ChunkSize, y*ChunkLoaderEntity.ChunkSize, 50f);
                //float[,] heightmap = new float[ChunkLoaderEntity.ChunkSize+1,ChunkLoaderEntity.ChunkSize+1];
                Tile[,] tiles = HeightMap.GenerateTileMap(heightmap);
            
                _chunkData = new ChunkData(x, y, 0, tiles);

                ChunkStatus = ChunkStatus.Unserialized;

                //un-comment these to enable chunk creation to harddrive.
                //Stream stream = File.Open("Content/Chunks/chunk_" + x + "_" + y + ".dat", FileMode.Create);
                //bf.Serialize(stream, _chunkData);
            }

            _chunkPositionX = x;
            _chunkPositionY = y;
            
            _heightMap = new HeightMap(_chunkData);
            
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
                _heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), _terrainTexture);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            _heightMap.Initialize(graphicsDevice);
        }

        public int GetChunkX()
        {
            return _chunkPositionX;
        }

        public int GetChunkY()
        {
            return _chunkPositionY;
        }

        public HeightMap GetHeightMap()
        {
            return _heightMap;
        }

        public void ChangeTileTexture(string name, int x, int y)
        {
            _chunkData.Tiles[x,y].TileTextureNameTriangleOne = name;
            _chunkData.Tiles[x,y].TileTextureNameTriangleTwo = name;

            _updatedHeightmap = true;
            ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeTriangleTexture(string name, int x, int y, bool alternativeTriangle)
        {
            if(alternativeTriangle)
                _chunkData.Tiles[x,y].TileTextureNameTriangleOne = name;
            else
                _chunkData.Tiles[x,y].TileTextureNameTriangleTwo = name;

            _updatedHeightmap = true;
            ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeColor(Color color, int x, int y)
        {
            _chunkData.Tiles[x,y].Color = color;
            _heightMap = new HeightMap(_chunkData);
            _heightMap.Initialize(_graphicsDevice);
            
            ChunkStatus = ChunkStatus.Changed;
        }

        public void ChangeVertexHeight(float height, int x, int y, TileCorner corner)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                {
                    _chunkData.Tiles[x, y].SetCornerHeight(height, TileCorner.TopLeft);
            
                    if(x > 0)
                        _chunkData.Tiles[x-1,y].SetCornerHeight(height, TileCorner.TopRight);
            
                    if(y > 0)
                        _chunkData.Tiles[x,y-1].SetCornerHeight(height, TileCorner.BottomLeft);
            
                    if(x > 0 && y > 0)
                        _chunkData.Tiles[x-1,y-1].SetCornerHeight(height, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.TopRight:
                {
                    _chunkData.Tiles[x,y].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(y > 0)
                        _chunkData.Tiles[x,y-1].SetCornerHeight(height, TileCorner.BottomRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y > 0)
                        _chunkData.Tiles[x+1,y-1].SetCornerHeight(height, TileCorner.BottomLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[x+1,y].SetCornerHeight(height, TileCorner.TopLeft);
                    break;
                }
                
                case TileCorner.BottomLeft:
                {
                    _chunkData.Tiles[x,y].SetCornerHeight(height, TileCorner.BottomLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[x,y+1].SetCornerHeight(height, TileCorner.TopLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1 && x > 0)
                        _chunkData.Tiles[x-1,y+1].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(x > 0)
                        _chunkData.Tiles[x-1,y].SetCornerHeight(height, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.BottomRight:
                {
                    _chunkData.Tiles[x,y].SetCornerHeight(height, TileCorner.BottomRight);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[x,y+1].SetCornerHeight(height, TileCorner.TopRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[x+1,y+1].SetCornerHeight(height, TileCorner.TopLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[x+1,y].SetCornerHeight(height, TileCorner.BottomLeft);
                    break;
                }
            }

            _updatedHeightmap = true;
            ChunkStatus = ChunkStatus.Changed;
        }

        public void UpdateHeightMap()
        {
            if (_updatedHeightmap)
            {
                _heightMap = new HeightMap(_chunkData);
                _heightMap.Initialize(_graphicsDevice);
                _updatedHeightmap = false;
            }
        }

        public float GetTileHeight(int x, int y)
        {
            return (_chunkData.Tiles[x, y].TopLeft
            +_chunkData.Tiles[x, y].TopRight
            +_chunkData.Tiles[x, y].BottomLeft
            +_chunkData.Tiles[x, y].BottomRight
                   )/4;
        }

        public float GetVertexHeight(int x, int y, TileCorner corner)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                    return _chunkData.Tiles[x, y].TopLeft;
                case TileCorner.TopRight:
                    return _chunkData.Tiles[x, y].TopRight;
                case TileCorner.BottomLeft:
                    return _chunkData.Tiles[x, y].BottomLeft;
                case TileCorner.BottomRight:
                    return _chunkData.Tiles[x, y].BottomRight;
                default:
                    return 0f;
            }
        }
    }
}