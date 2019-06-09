using System;
using System.Collections.Generic;
using System.IO;
using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Type.Enum;
using DProject.Type.Rendering;
using DProject.Type.Serializable.Chunk;
using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using Object = DProject.Type.Serializable.Chunk.Object;

namespace DProject.Entity.Chunk
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize
    {
        private HeightMap[] _heightMaps;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        private readonly ChunkData _chunkData;

        private List<PropEntity>[] _props;
        private BoundingBox[][,] _tileBoundingBoxes;
        
        private readonly BoundingSphere _boundingSphere;
        
        public enum TileCorner { TopLeft, TopRight, BottomLeft, BottomRight }
        
        public TerrainEntity(int x, int y) : base(new Vector3(x*ChunkLoaderEntity.ChunkSize, 0, y*ChunkLoaderEntity.ChunkSize), Quaternion.Identity, new Vector3(1,1,1))
        {
            _chunkPositionX = x;
            _chunkPositionY = y;

            _chunkData = GenerateChunkData(_chunkPositionX, _chunkPositionY);
            
            LoadProps();
            
            _heightMaps = new HeightMap[_chunkData.GetFloorCount()];
            
            for (var floor = 0; floor < _heightMaps.Length; floor++)
                _heightMaps[floor] = new HeightMap(_chunkData.Tiles[floor]);
            
            _boundingSphere = new BoundingSphere(new Vector3(ChunkLoaderEntity.ChunkSize/2, 0, ChunkLoaderEntity.ChunkSize/2), ChunkLoaderEntity.ChunkSize/1.6f);
        }

        public static ChunkData GenerateChunkData(int x, int y)
        {
            string path = "Content/chunks/chunk_" + x + "_" + y + ".dat";
            ChunkData chunkdata;
            
            if (File.Exists(path))
            {
                Stream stream = File.Open(path, FileMode.Open);
                var bytes = stream;
                
                chunkdata = LZ4MessagePackSerializer.Deserialize<ChunkData>(bytes);
                stream.Close();

                chunkdata.ChunkStatus = ChunkStatus.Current;
            }
            else
            {                   
                byte[,] heightmap = Noise.GenerateNoiseMap(ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, x*ChunkLoaderEntity.ChunkSize, y*ChunkLoaderEntity.ChunkSize, 50f);
                
                Tile[][,] tiles = new Tile[1][,];
                
                tiles[0] = HeightMap.GenerateTileMap(heightmap);
                
                chunkdata = new ChunkData()
                {
                    ChunkPositionX = x,
                    ChunkPositionY = y,
                    Tiles = tiles,
                    
                    Objects = Object.GenerateObjects(0, 0, 1, 1),
                    
                    LightingInfo = LightingProperties.DefaultInfo,
                    
                    ChunkStatus = ChunkStatus.Unserialized
                };
            }

            return chunkdata;
        }

        private BoundingBox[][,] GenerateBoundingBoxes()
        {
            var floorCount = _chunkData.GetFloorCount();
            var xSize = _chunkData.Tiles[0].GetLength(0);
            var ySize = _chunkData.Tiles[0].GetLength(1);
            
            BoundingBox[][,] boundingBoxes = new BoundingBox[floorCount][,];

            for (int i = 0; i < boundingBoxes.Length; i++)
                boundingBoxes[i] = new BoundingBox[xSize,ySize];
            
            for (int floor = 0; floor < floorCount; floor++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        boundingBoxes[floor][x,y] = BoundingBox.CreateFromPoints
                        (new []{
                            new Vector3(x-0.5f, _chunkData.Tiles[floor][x,y].TopLeft/HeightMap.IncrementsPerHeightUnit, y-0.5f) + GetPosition(),
                            new Vector3(x+0.5f, _chunkData.Tiles[floor][x,y].TopRight/HeightMap.IncrementsPerHeightUnit, y-0.5f) + GetPosition(),
                            new Vector3(x-0.5f, _chunkData.Tiles[floor][x,y].BottomLeft/HeightMap.IncrementsPerHeightUnit, y+0.5f) + GetPosition(),
                            new Vector3(x+0.5f, _chunkData.Tiles[floor][x,y].BottomRight/HeightMap.IncrementsPerHeightUnit, y+0.5f) + GetPosition(),
                        });
                    }
                }
            }

            return boundingBoxes;
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
            
            foreach (var floor in _props)
            {
                foreach (var prop in floor)
                    prop.LoadContent(content);
            }
        }

        public static void Serialize(ChunkData chunkData)
        {
            chunkData.ChunkStatus = ChunkStatus.Current;

            Stream stream = File.Open("Content/chunks/chunk_" + chunkData.ChunkPositionX + "_" + chunkData.ChunkPositionY + ".dat", FileMode.Create);
            var bytes = LZ4MessagePackSerializer.Serialize(chunkData);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Serialize()
        {
            Serialize(_chunkData);
        }

        public void Draw(CameraEntity activeCamera)
        {           
            if (activeCamera.GetBoundingFrustum().Intersects(_boundingSphere.Transform(GetWorldMatrix())))
            {
                foreach (var heightMap in _heightMaps)
                {
                    heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), Textures.TerrainTexture);
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
            {
                heightMap.Initialize(graphicsDevice);
            }
            
            _tileBoundingBoxes = GenerateBoundingBoxes();
        }

        private void LoadProps()
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
                        floor)/HeightMap.IncrementsPerHeightUnit,
                    _chunkData.Objects[floor][index].PositionY + _chunkPositionY *ChunkLoaderEntity.ChunkSize),
                CalculateRotation(_chunkData.Objects[floor][index].ObjectRotation),
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
                        floor)/HeightMap.IncrementsPerHeightUnit,
                    propObject.PositionY + _chunkPositionY *ChunkLoaderEntity.ChunkSize),
                CalculateRotation(propObject.ObjectRotation),
                propObject.Scale,
                propObject.Id
            ); 
        }

        private static Quaternion CalculateRotation(Rotation rotation)
        {
            switch (rotation)
            {
                case Type.Enum.Rotation.North:
                    return Quaternion.Identity;
                case Type.Enum.Rotation.East:
                    return new Quaternion(0f, -0.707107f, 0f, 0.707107f);
                case Type.Enum.Rotation.South:
                    return new Quaternion(0f, 1f, 0f, 0f);
                case Type.Enum.Rotation.West:
                    return new Quaternion(0f, 0.707107f, 0f, 0.707107f);
                default:
                    return Quaternion.Identity;
            }
        }

        public void UpdateHeightMap()
        {
            for (int floor = 0; floor < _heightMaps.Length; floor++)
            {
                if (_heightMaps[floor].GetHasUpdated())
                    _heightMaps[floor].UpdateHeightMap(_chunkData.Tiles[floor]);
            }
        }
        
        #region Editing
        
        public void PlaceProp(int x, int y, int floor, Rotation rotation, ushort objectId)
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
                        
                        ObjectRotation = rotation,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f
                    };

                    _props[floor][i] = PropEntityFromObject(_chunkData.Objects[floor][i], floor);
                    _props[floor][i].LoadContent(_contentManager);
                    
                    existingProp = true;
                    _chunkData.ChunkStatus = ChunkStatus.Changed;
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

                    ObjectRotation = rotation,
                    ScaleX = 1f,
                    ScaleY = 1f,
                    ScaleZ = 1f
                };
                
                _chunkData.Objects[floor].Add(propObject);

                var prop = PropEntityFromObject(propObject, floor);
                
                _props[floor].Add(prop);
                prop.LoadContent(_contentManager);
                
                _chunkData.ChunkStatus = ChunkStatus.Changed;
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
                    _chunkData.ChunkStatus = ChunkStatus.Changed;
                    return;
                }
            }
        }

        public void ChangeTileTexture(ushort? textureId, int x, int y, int floor)
        {
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleOne = textureId;
            _chunkData.Tiles[floor][x,y].TileTextureIdTriangleTwo = textureId;

            _heightMaps[floor].SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeTriangleTexture(ushort? textureId, int x, int y, int floor, bool alternativeTriangle)
        {
            if(alternativeTriangle)
                _chunkData.Tiles[floor][x,y].TileTextureIdTriangleOne = textureId;
            else
                _chunkData.Tiles[floor][x,y].TileTextureIdTriangleTwo = textureId;

            _heightMaps[floor].SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeCornerColor(ushort colorId, TileCorner corner, int x, int y, int floor)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                    _chunkData.Tiles[floor][x, y].ColorTopLeft = colorId;
                    break;
                case TileCorner.TopRight:
                    _chunkData.Tiles[floor][x, y].ColorTopRight = colorId;
                    break;
                case TileCorner.BottomLeft:
                    _chunkData.Tiles[floor][x, y].ColorBottomLeft = colorId;
                    break;
                case TileCorner.BottomRight:
                    _chunkData.Tiles[floor][x, y].ColorBottomRight = colorId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }

            _heightMaps[floor].SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeVertexColor(ushort color, int x, int y, int floor, TileCorner corner)
        {
            switch (corner)
            {
                case TileCorner.TopLeft:
                {
                    _chunkData.Tiles[floor][x, y].SetCornerColor(color, TileCorner.TopLeft);
            
                    if(x > 0)
                        _chunkData.Tiles[floor][x-1,y].SetCornerColor(color, TileCorner.TopRight);
            
                    if(y > 0)
                        _chunkData.Tiles[floor][x,y-1].SetCornerColor(color, TileCorner.BottomLeft);
            
                    if(x > 0 && y > 0)
                        _chunkData.Tiles[floor][x-1,y-1].SetCornerColor(color, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.TopRight:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerColor(color, TileCorner.TopRight);
                
                    if(y > 0)
                        _chunkData.Tiles[floor][x,y-1].SetCornerColor(color, TileCorner.BottomRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y > 0)
                        _chunkData.Tiles[floor][x+1,y-1].SetCornerColor(color, TileCorner.BottomLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y].SetCornerColor(color, TileCorner.TopLeft);
                    break;
                }
                
                case TileCorner.BottomLeft:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerColor(color, TileCorner.BottomLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x,y+1].SetCornerColor(color, TileCorner.TopLeft);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1 && x > 0)
                        _chunkData.Tiles[floor][x-1,y+1].SetCornerColor(color, TileCorner.TopRight);
                
                    if(x > 0)
                        _chunkData.Tiles[floor][x-1,y].SetCornerColor(color, TileCorner.BottomRight);
                    break;
                }
                
                case TileCorner.BottomRight:
                {
                    _chunkData.Tiles[floor][x,y].SetCornerColor(color, TileCorner.BottomRight);
                
                    if(y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x,y+1].SetCornerColor(color, TileCorner.TopRight);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1 && y < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y+1].SetCornerColor(color, TileCorner.TopLeft);
                
                    if(x < ChunkLoaderEntity.ChunkSize-1)
                        _chunkData.Tiles[floor][x+1,y].SetCornerColor(color, TileCorner.BottomLeft);
                    break;
                }
            }

            _heightMaps[floor].SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }
        
        public void ChangeTileColor(ushort colorId, int x, int y, int floor)
        {
            _chunkData.Tiles[floor][x, y].ColorTopLeft = colorId;
            _chunkData.Tiles[floor][x, y].ColorTopRight = colorId;
            _chunkData.Tiles[floor][x, y].ColorBottomLeft = colorId;
            _chunkData.Tiles[floor][x, y].ColorBottomRight = colorId;

            _heightMaps[floor].SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
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
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }

        #endregion

        #region Getters and Setters
        
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

        public BoundingBox[,] GetTileBoundingBoxes(int floor)
        {
            return _tileBoundingBoxes[floor];
        }

        public BoundingBox GetTileBoundingBox(int x, int y, int floor)
        {
            return _tileBoundingBoxes[floor][x,y];
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

        public ChunkData GetChunkData()
        {
            return _chunkData;
        }

        #endregion
    }
}