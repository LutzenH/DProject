using System;
using System.Collections.Generic;
using System.Threading;
using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Chunk
{
    public class ChunkLoaderEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private readonly EntityManager _entityManager;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        private Vector2 _previousChunkPosition;
        private Vector2 _chunkPosition;

        private TerrainEntity[,] _loadedChunks = new TerrainEntity[3,3];

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
            {
                LoadChunks(_chunkPosition);
            }

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

            List<Vector2> newChunkPositions = new List<Vector2>();
            List<Vector2> newChunkLocations = new List<Vector2>();
            
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
                    {
                        newChunkPositions.Add(new Vector2(x,y));
                        newChunkLocations.Add(new Vector2(position.X, position.Y));
                    }
                }
            }
                                    
            Thread thread = new Thread((() => LoadNewChunks(newChunkPositions, newChunkLocations)));
            thread.Start();
        }

        private void LoadNewChunks(List<Vector2> newChunkPositions, List<Vector2> newChunkLocations)
        {
            for (int i = 0; i < newChunkPositions.Count; i++)
            {
                _loadedChunks[(int) newChunkPositions[i].X, (int) newChunkPositions[i].Y] = LoadChunk((int)newChunkLocations[i].X, (int)newChunkLocations[i].Y);
            }
        }
        
        private TerrainEntity LoadChunk(int x, int y)
        {
            TerrainEntity chunk = new TerrainEntity(x, y);
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
                if (chunk != null)
                {
                    if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                    {
                        return chunk.GetTileHeight(localChunkPositionX, localChunkPositionY);
                    }
                }
            }

            return null;
        }

        public static Vector2 GetLocalChunkPosition(Vector2 position)
        {
            Vector2 tempPosition = new Vector2((float)Math.Floor(position.X+0.5f), (float)Math.Floor(position.Y+0.5f));
            
            int chunkPositionX = (int)Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int)Math.Floor(tempPosition.Y / ChunkSize);
            
            int localChunkPositionX = (int)tempPosition.X - (chunkPositionX*ChunkSize);
            int localChunkPositionY = (int)tempPosition.Y - (chunkPositionY*ChunkSize);
            
            return new Vector2(localChunkPositionX, localChunkPositionY);
        }

        public TerrainEntity GetChunk(Vector3 position)
        {
            Vector2 tempPosition = new Vector2((float)Math.Floor(position.X+0.5f), (float)Math.Floor(position.Z+0.5f));
            
            int chunkPositionX = (int)Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int)Math.Floor(tempPosition.Y / ChunkSize);
            
            foreach (var chunk in _loadedChunks)
            {
                if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                {
                    return chunk;
                }
            }

            return null;
        }

        public TerrainEntity[,] GetLoadedChunks()
        {
            return _loadedChunks;
        }
    }
}