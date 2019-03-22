using System;
using System.Collections.Generic;
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
using Object = DProject.Type.Serializable.Object;

namespace DProject.Entity.Chunk
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize
    {
        private HeightMap[] _heightMaps;
        private Texture2D _terrainTexture;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        private readonly ChunkData _chunkData;

        private List<PropEntity>[] _props;
        
        private readonly BoundingSphere _boundingSphere;
        
        public ChunkStatus ChunkStatus { get; set; }

        public enum TileCorner { TopLeft, TopRight, BottomLeft, BottomRight }

        private const int DefaultFloorCount = 2;
        
        public TerrainEntity(int x, int y) : base(new Vector3(x*ChunkLoaderEntity.ChunkSize, 0, y*ChunkLoaderEntity.ChunkSize), Quaternion.Identity, new Vector3(1,1,1))
        {
            string path = "Content/chunks/chunk_" + x + "_" + y + ".dat";
            
            _chunkPositionX = x;
            _chunkPositionY = y;
            
            if (File.Exists(path))
            {
                Stream stream = File.Open("Content/chunks/chunk_" + x + "_" + y + ".dat", FileMode.Open);
                var bytes = stream;
                
                _chunkData = LZ4MessagePackSerializer.Deserialize<ChunkData>(bytes);
                stream.Close();

                SetProps();
                
                ChunkStatus = ChunkStatus.Current;
            }
            else
            {                   
                byte[,] heightmap = Noise.GenerateNoiseMap(ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, x*ChunkLoaderEntity.ChunkSize, y*ChunkLoaderEntity.ChunkSize, 50f);
                
                Tile[][,] tiles = new Tile[DefaultFloorCount][,];
                
                tiles[0] = HeightMap.GenerateTileMap(heightmap);
                tiles[1] = HeightMap.GenerateTileMap(ChunkLoaderEntity.ChunkSize, 1);
                
                _chunkData = new ChunkData()
                {
                    ChunkPositionX = x,
                    ChunkPositionY = y,
                    Tiles = tiles,
                    
                    Objects = Object.GenerateObjects(0, 0, DefaultFloorCount, 10)
                };

                SetProps();
                
                ChunkStatus = ChunkStatus.Unserialized;

                //un-comment these to enable chunk creation to harddrive.
                //Stream stream = File.Open("Content/chunks/chunk_" + x + "_" + y + ".dat", FileMode.Create);
                //var bytes = LZ4MessagePackSerializer.Serialize(_chunkData);
                //stream.Write(bytes, 0, bytes.Length);
            }

            _heightMaps = new HeightMap[DefaultFloorCount];
            
            for (var floor = 0; floor < _heightMaps.Length; floor++)
                _heightMaps[floor] = new HeightMap(_chunkData.Tiles[floor]);
            
            _boundingSphere = new BoundingSphere(new Vector3(x + ChunkLoaderEntity.ChunkSize/2, 0, y + ChunkLoaderEntity.ChunkSize/2), ChunkLoaderEntity.ChunkSize/1.6f);
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
            
            _terrainTexture = content.Load<Texture2D>(Textures.TextureAtlasLocation);

            foreach (var floor in _props)
            {
                foreach (var prop in floor)
                    prop.LoadContent(content);
            }
        }

        public void Draw(CameraEntity activeCamera)
        {           
            if (activeCamera.GetBoundingFrustum().Intersects(_boundingSphere.Transform(GetWorldMatrix())))
            {
                foreach (var heightMap in _heightMaps)
                {
                    heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), _terrainTexture);
                }
                
                foreach (var floor in _props)
                {
                    foreach (var prop in floor)
                        prop.Draw(activeCamera);
                }
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            foreach (var heightMap in _heightMaps)
                heightMap.Initialize(graphicsDevice);
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

        private void SetProps()
        {
            _props = new List<PropEntity>[_chunkData.Objects.Length];
                
            for(var i = 0; i < _props.Length; i++)
                _props[i] = new List<PropEntity>();
            
            for (var floor = 0; floor < _props.Length; floor++)
            {                
                for (var i = 0; i < _chunkData.Objects[floor].Count; i++)
                {
                    _props[floor].Add(PropEntityFromObject(floor, i));
                }
            }
        }

        private PropEntity PropEntityFromObject(int floor, int index)
        {
            return new PropEntity(
                new Vector3(
                    _chunkData.Objects[floor][index].PositionX + _chunkPositionX *ChunkLoaderEntity.ChunkSize,
                    GetTileHeight(
                        _chunkData.Objects[floor][index].PositionX,
                        _chunkData.Objects[floor][index].PositionY,
                        floor)/4f,
                    _chunkData.Objects[floor][index].PositionY + _chunkPositionY *ChunkLoaderEntity.ChunkSize),
                Quaternion.Identity,
                _chunkData.Objects[floor][index].Scale,
                _chunkData.Objects[floor][index].Id
            );
        }

        private PropEntity PropEntityFromObject(Object propObject, int floor)
        {
            return new PropEntity(
                new Vector3(
                    propObject.PositionX + _chunkPositionX *ChunkLoaderEntity.ChunkSize,
                    GetTileHeight(
                        propObject.PositionX,
                        propObject.PositionY,
                        floor)/4f,
                    propObject.PositionY + _chunkPositionY *ChunkLoaderEntity.ChunkSize),
                Quaternion.Identity,
                propObject.Scale,
                propObject.Id
            ); 
        }

        public void PlaceProp(int x, int y, int floor, ushort objectId)
        {
            var existingProp = false;
            
            for (var i = 0; i < _chunkData.Objects[floor].Count; i++)
            {
                if (_chunkData.Objects[floor][i].PositionX == x && _chunkData.Objects[floor][i].PositionY == y)
                {
                    _chunkData.Objects[floor][i] = new Object()
                    {
                        Id = objectId,
                        PositionX = x,
                        PositionY = y,
                        
                        Rotation = 0,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f
                    };

                    _props[floor][i] = PropEntityFromObject(_chunkData.Objects[floor][i], floor);
                    _props[floor][i].LoadContent(_contentManager);
                    
                    existingProp = true;
                    ChunkStatus = ChunkStatus.Changed;
                    return;
                }
            }

            if (!existingProp)
            {
                var propObject = new Object()
                {
                    Id = objectId,
                    PositionX = x,
                    PositionY = y,

                    Rotation = 0,
                    ScaleX = 1f,
                    ScaleY = 1f,
                    ScaleZ = 1f
                };
                
                _chunkData.Objects[floor].Add(propObject);

                var prop = PropEntityFromObject(propObject, floor);
                
                _props[floor].Add(prop);
                prop.LoadContent(_contentManager);
                
                ChunkStatus = ChunkStatus.Changed;
            }
        }

        public void RemoveProp(int x, int y, int floor)
        {
            for (var i = 0; i < _chunkData.Objects[floor].Count; i++)
            {
                if (_chunkData.Objects[floor][i].PositionX == x && _chunkData.Objects[floor][i].PositionY == y)
                {
                    _chunkData.Objects[floor].RemoveAt(i);
                    _props[floor].RemoveAt(i);
                    ChunkStatus = ChunkStatus.Changed;
                    return;
                }
            }
        }

        public void ChangeTileTexture(ushort? textureId, int x, int y, int floor)
        {
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleOne = textureId;
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleTwo = textureId;

            _heightMaps[floor].SetHasUpdated(true);
            ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeTriangleTexture(ushort? textureId, int x, int y, int floor, bool alternativeTriangle)
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

        public void ChangeVertexHeight(byte height, int x, int y, int floor, TileCorner corner)
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

        public byte GetTileHeight(int x, int y, int floor)
        {
            return (byte) Math.Round((_chunkData.Tiles[floor][x, y].TopLeft
                                    +_chunkData.Tiles[floor][x, y].TopRight
                                    +_chunkData.Tiles[floor][x, y].BottomLeft
                                    +_chunkData.Tiles[floor][x, y].BottomRight)
                           /4f);
        }

        public byte GetVertexHeight(int x, int y, int floor, TileCorner corner)
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
                    return 0;
            }
        }

        public List<PropEntity>[] GetProps()
        {
            return _props;
        }

        public List<PropEntity> GetProps(int floor)
        {
            return _props[floor];
        }
    }
}