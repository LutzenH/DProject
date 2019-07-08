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
        private readonly HeightMap _heightMap;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private ContentManager _contentManager;
        
        private readonly ChunkData _chunkData;

        private List<PropEntity> _props;
        private BoundingBox[,] _tileBoundingBoxes;
        
        private readonly BoundingBox _boundingBox;
        
        public enum TileCorner { TopLeft, TopRight, BottomLeft, BottomRight }
        
        public TerrainEntity(int x, int y, LevelOfDetail levelOfDetail) : base(new Vector3(x * ChunkLoaderEntity.ChunkSize, 0, y * ChunkLoaderEntity.ChunkSize), Quaternion.Identity, new Vector3(1,1,1))
        {
            _chunkPositionX = x;
            _chunkPositionY = y;

            _chunkData = GenerateChunkData(_chunkPositionX, _chunkPositionY);
            
            LoadProps();
            
            _heightMap = new HeightMap(_chunkData.VertexMap, levelOfDetail);

            var (lowest, highest) = _heightMap.GetOuterHeightBounds();
            
            _boundingBox = BoundingBox.CreateFromPoints
            (new []{
                new Vector3(x * ChunkLoaderEntity.ChunkSize, highest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize, highest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize, highest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize, highest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize),
                
                new Vector3(x * ChunkLoaderEntity.ChunkSize, lowest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize, lowest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize, lowest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize),
                new Vector3(x * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize, lowest / HeightMap.IncrementsPerHeightUnit, y * ChunkLoaderEntity.ChunkSize + ChunkLoaderEntity.ChunkSize)
            });
        }

        public static ChunkData GenerateChunkData(int x, int y)
        {
            var path = "Content/chunks/chunk_" + x + "_" + y + ".dat";
            ChunkData chunkData;
            
            if (File.Exists(path))
            {
                Stream stream = File.Open(path, FileMode.Open);
                var bytes = stream;
                
                chunkData = LZ4MessagePackSerializer.Deserialize<ChunkData>(bytes);
                stream.Close();

                chunkData.ChunkStatus = ChunkStatus.Current;
            }
            else
            {                   
                ushort[,] shortMap = Noise.GenerateNoiseMap(ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, x*ChunkLoaderEntity.ChunkSize, y*ChunkLoaderEntity.ChunkSize, 50f);
                
                var vertices = HeightMap.GenerateVertexMap(shortMap);
                
                chunkData = new ChunkData()
                {
                    ChunkPositionX = x,
                    ChunkPositionY = y,
                    VertexMap = vertices,
                    
                    Objects = Object.GenerateObjects(0, 0, 0),
                    
                    SkyId = Skies.GetDefaultSkyId(),
                    
                    ChunkStatus = ChunkStatus.Unserialized
                };
            }

            return chunkData;
        }

        private BoundingBox[,] GenerateBoundingBoxes()
        {
            var xSize = _chunkData.VertexMap.GetLength(0) - 1;
            var ySize = _chunkData.VertexMap.GetLength(1) - 1;
            
            var boundingBoxes = new BoundingBox[xSize, ySize];

            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    boundingBoxes[x, y] = BoundingBox.CreateFromPoints
                    (new []{
                        new Vector3(x, _chunkData.VertexMap[x, y].Height/HeightMap.IncrementsPerHeightUnit, y) + GetPosition(),
                        new Vector3(x + 1, _chunkData.VertexMap[x + 1, y].Height/HeightMap.IncrementsPerHeightUnit, y) + GetPosition(),
                        new Vector3(x, _chunkData.VertexMap[x, y + 1].Height/HeightMap.IncrementsPerHeightUnit, y + 1) + GetPosition(),
                        new Vector3(x + 1, _chunkData.VertexMap[x + 1, y + 1].Height/HeightMap.IncrementsPerHeightUnit, y + 1) + GetPosition(),
                    });
                }
            }

            return boundingBoxes;
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
            
            foreach (var prop in _props)
                prop.LoadContent(content);
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
        
        public static void SerializeChunkDataList(List<ChunkData> chunkData)
        {
            foreach (var chunk in chunkData) 
                Serialize(chunk);
        }

        public void Draw(CameraEntity activeCamera)
        {           
            if (activeCamera.GetBoundingFrustum().Intersects(_boundingBox))
            {
                _heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), Textures.TerrainTexture);

                foreach (var prop in _props)
                    prop.Draw(activeCamera);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _heightMap.Initialize(graphicsDevice);

            _tileBoundingBoxes = GenerateBoundingBoxes();
        }

        private void LoadProps()
        {
            _props = new List<PropEntity>();

            for (var i = 0; i < _chunkData.Objects.Count; i++)
                _props.Add(PropEntityFromObject(i));
        }

        private PropEntity PropEntityFromObject(int index)
        {
            return PropEntityFromObject(_chunkData.Objects[index]);
        }

        private PropEntity PropEntityFromObject(Object propObject)
        {
            return new PropEntity(
                new Vector3(
                    propObject.PositionX + _chunkPositionX *ChunkLoaderEntity.ChunkSize,
                    GetTileHeight(propObject.PositionX, propObject.PositionY) / HeightMap.IncrementsPerHeightUnit,
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
            if (_heightMap.GetHasUpdated())
                _heightMap.UpdateHeightMap(_chunkData.VertexMap);
        }

        public void SetLevelOfDetail(LevelOfDetail levelOfDetail)
        {
            _heightMap.SetLevelOfDetail(_chunkData.VertexMap, levelOfDetail);
        }

        #region Editing
        
        public void PlaceProp(int x, int y, Rotation rotation, ushort objectId)
        {
            for (var i = 0; i < _chunkData.Objects.Count; i++)
            {
                if (_chunkData.Objects[i].PositionX == x && _chunkData.Objects[i].PositionY == y)
                {
                    _chunkData.Objects[i] = new Object()
                    {
                        Id = objectId,
                        PositionX = x,
                        PositionY = y,
                        
                        ObjectRotation = rotation,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f
                    };

                    _props[i] = PropEntityFromObject(_chunkData.Objects[i]);
                    _props[i].LoadContent(_contentManager);
                    
                    _chunkData.ChunkStatus = ChunkStatus.Changed;
                    
                    return;
                }
            }

            //If it did not return:
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
                
                _chunkData.Objects.Add(propObject);

                var prop = PropEntityFromObject(propObject);
                
                _props.Add(prop);
                prop.LoadContent(_contentManager);
                
                _chunkData.ChunkStatus = ChunkStatus.Changed;
            }
        }

        public void RemoveProp(int x, int y)
        {
            for (var i = 0; i < _chunkData.Objects.Count; i++)
            {
                if (_chunkData.Objects[i].PositionX == x && _chunkData.Objects[i].PositionY == y)
                {
                    _chunkData.Objects.RemoveAt(i);
                    _props.RemoveAt(i);
                    _chunkData.ChunkStatus = ChunkStatus.Changed;
                    return;
                }
            }
        }

        public void ChangeVertexTexture(ushort? textureId, int x, int y)
        {
            _chunkData.VertexMap[x,y].TextureId = textureId;

            _heightMap.SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }

        public void ChangeVertexColor(ushort color, int x, int y)
        {
            _chunkData.VertexMap[x, y].ColorId = color;

            _heightMap.SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }

        public void ChangeVertexHeight(ushort height, int x, int y)
        {
            _chunkData.VertexMap[x, y].Height = height;

            _heightMap.SetHasUpdated(true);
            _chunkData.ChunkStatus = ChunkStatus.Changed;
        }

        #endregion

        #region Getters and Setters
        
        public ushort GetTileHeight(int x, int y)
        {
            return (ushort) Math.Round((_chunkData.VertexMap[x, y].Height
                                    +_chunkData.VertexMap[x + 1, y].Height
                                    +_chunkData.VertexMap[x, y + 1].Height
                                    +_chunkData.VertexMap[x + 1, y + 1].Height)
                           /4f);
        }

        public ushort GetVertexHeight(int x, int y)
        {
            return _chunkData.VertexMap[x, y].Height;
        }

        public List<PropEntity> GetProps()
        {
            return _props;
        }

        public BoundingBox[,] GetTileBoundingBoxes()
        {
            return _tileBoundingBoxes;
        }

        public BoundingBox GetTileBoundingBox(int x, int y)
        {
            return _tileBoundingBoxes[x, y];
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

        public ChunkData GetChunkData()
        {
            return _chunkData;
        }

        #endregion
    }
}