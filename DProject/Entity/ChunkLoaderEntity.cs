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
            TerrainEntity chunk = new TerrainEntity(x, y, _chunkSize, 50f);
            chunk.Initialize(_graphicsDevice);
            chunk.LoadContent(_contentManager);

            return chunk;
        }
    }
}