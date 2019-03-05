using System;
using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class ChunkLoaderEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private readonly EntityManager _entityManager;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        private Vector2 _previousChunkPosition;
        private Vector2 _chunkPosition;

        private TerrainEntity[,] _loadedChunks = new TerrainEntity[5,5];

        public const int ChunkSize = 64;
        
        public ChunkLoaderEntity(EntityManager entityManager) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _entityManager = entityManager;
            _chunkPosition = new Vector2(0,0);
            _previousChunkPosition = new Vector2(-1, 0);
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
        }

        public void Update(GameTime gameTime)
        {
            _chunkPosition = CalculateChunkPosition(_entityManager.GetActiveCamera().GetPosition().X, _entityManager.GetActiveCamera().GetPosition().Z);

            if (!_chunkPosition.Equals(_previousChunkPosition))
                LoadChunks(_chunkPosition);

            _previousChunkPosition = _chunkPosition;
        }

        public void Draw(CameraEntity activeCamera)
        {
            foreach (var chunk in _loadedChunks)
                chunk?.Draw(activeCamera);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private void LoadChunks(Vector2 chunkPosition)
        {   
            TerrainEntity[,] oldChunks = _loadedChunks;
            
            _loadedChunks = new TerrainEntity[_loadedChunks.GetLength(0),_loadedChunks.GetLength(1)];
            
            for (int x = 0; x < _loadedChunks.GetLength(0); x++)
            {
                for (int y = 0; y < _loadedChunks.GetLength(1); y++)
                {
                    Vector2 position = chunkPosition - new Vector2(x-_loadedChunks.GetLength(0)/2, y-_loadedChunks.GetLength(1)/2);

                    foreach (var chunk in oldChunks)
                    {
                        if (chunk != null)
                        {
                            if (chunk.GetChunkX() == (int) position.X && chunk.GetChunkY() == (int) position.Y)
                                _loadedChunks[x, y] = chunk;
                        }
                    }

                    if (_loadedChunks[x, y] == null)
                        _loadedChunks[x, y] = LoadChunk((int)position.X, (int)position.Y);
                }
            }
        }

        private TerrainEntity LoadChunk(int x, int y)
        {
            TerrainEntity chunk = new TerrainEntity(x, y, ChunkSize, 50f);
            chunk.Initialize(_graphicsDevice);
            chunk.LoadContent(_contentManager);

            return chunk;
        }

        public static Vector2 CalculateChunkPosition(float x, float y)
        {
            return new Vector2(
                (int)(x - (ChunkSize/2)) / ChunkSize,
                (int)(y - (ChunkSize/2)) / ChunkSize);
        }

        public float? GetHeightFromPosition(Vector2 position)
        {
            Vector2 tempPosition = new Vector2((float)Math.Floor(position.X+0.5f), (float)Math.Floor(position.Y+0.5f));
            
            int chunkPositionX = (int)Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int)Math.Floor(tempPosition.Y / ChunkSize);
            
            int localChunkPositionX = (int)tempPosition.X - (chunkPositionX*ChunkSize);
            int localChunkPositionY = (int)tempPosition.Y - (chunkPositionY*ChunkSize);

            if (localChunkPositionX < 0)
                localChunkPositionX = ChunkSize + localChunkPositionX;
            
            if (localChunkPositionY < 0)
                localChunkPositionY = ChunkSize + localChunkPositionY;
            
            foreach (var chunk in _loadedChunks)
            {
                if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                {                                
                    return (chunk.GetHeightMap().GetHeightMap()[localChunkPositionX, localChunkPositionY]
                            + chunk.GetHeightMap().GetHeightMap()[localChunkPositionX+1, localChunkPositionY]
                            + chunk.GetHeightMap().GetHeightMap()[localChunkPositionX, localChunkPositionY+1]
                            + chunk.GetHeightMap().GetHeightMap()[localChunkPositionX+1, localChunkPositionY+1]) / 4;
                }
            }

            return null;
        }
    }
}