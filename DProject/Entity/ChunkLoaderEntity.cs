using System;
using DProject.List;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Entity
{
    public class ChunkLoaderEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private readonly EntityManager _entityManager;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        private Vector2 _previousChunkPosition;
        private Vector2 _chunkPosition;

        private readonly TerrainEntity[,] _loadedChunks = new TerrainEntity[3,3];

        private int _chunkSize = 64;
        
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
            _chunkPosition = new Vector2(
                (int)(_entityManager.GetActiveCamera().GetPosition().X - (_chunkSize/2)) / _chunkSize,
                (int)(_entityManager.GetActiveCamera().GetPosition().Z - (_chunkSize/2)) / _chunkSize
                );

            if (!_chunkPosition.Equals(_previousChunkPosition))
            {
                Console.WriteLine("ChunkPosition changed: " + _chunkPosition);
                
                LoadChunks(_chunkPosition);
            }

            _previousChunkPosition = _chunkPosition;
        }

        public void Draw(CameraEntity activeCamera)
        {
            foreach (var chunk in _loadedChunks)
            {
                if(chunk != null)
                    chunk.Draw(activeCamera);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private void LoadChunks(Vector2 chunkPosition)
        {            
            for (int x = 0; x < _loadedChunks.GetLength(0); x++)
            {
                for (int y = 0; y < _loadedChunks.GetLength(1); y++)
                {
                    Vector2 tempPosition = chunkPosition - new Vector2(x-1, y-1);                    
                    
                    _loadedChunks[x, y] = LoadChunk(tempPosition);
                }
            }
        }

        private TerrainEntity LoadChunk(Vector2 position)
        {
            Vector2 tempPosition = new Vector2(position.X * _chunkSize, position.Y*_chunkSize);

            TerrainEntity chunk = new TerrainEntity(new Vector3(tempPosition.X,0,tempPosition.Y), _chunkSize, _chunkSize, 50f);
            chunk.Initialize(_graphicsDevice);
            chunk.LoadContent(_contentManager);

            return chunk;
        }
    }
}