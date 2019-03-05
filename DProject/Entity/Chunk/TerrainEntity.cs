using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Chunk
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private readonly HeightMap _heightMap;
        private Texture2D _terrainTexture;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;

        private readonly ChunkData _chunkData;

        public ChunkStatus ChunkStatus { get; set; }

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
                Tile[,] tiles = HeightMap.GenerateTileMap(heightmap);
            
                _chunkData = new ChunkData(
                    x,
                    y,
                    0,
                    tiles
                );

                ChunkStatus = ChunkStatus.Unserialized;

                //un-comment these to enable chunk creation to harddrive.
                //Stream stream = File.Open("Content/Chunks/chunk_" + x + "_" + y + ".dat", FileMode.Create);
                //bf.Serialize(stream, _chunkData);
            }

            _chunkPositionX = x;
            _chunkPositionY = y;
            
            _heightMap = new HeightMap(_chunkData);
        }

        public override void LoadContent(ContentManager content)
        {
            _terrainTexture = content.Load<Texture2D>(Textures.TextureAtlasLocation);
        }

        public void Draw(CameraEntity activeCamera)
        {
            _heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), _terrainTexture);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _heightMap.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime) {}

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

        public float GetTileHeight(int x, int y)
        {
            return (_chunkData.Tiles[x, y].TopLeft
            +_chunkData.Tiles[x, y].TopRight
            +_chunkData.Tiles[x, y].BottomLeft
            +_chunkData.Tiles[x, y].BottomRight
                   )/4;
        }
    }
}