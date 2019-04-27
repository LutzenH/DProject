using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using DProject.Type.Rendering.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class WorldMapUI : AbstractUI, IInitialize, IUpdateable
    {
        private readonly ChunkLoaderEntity _chunkLoaderEntity;

        private GraphicsDevice _graphicsDevice;

        private readonly MapChunkTexture2D[,] _mapChunkTexture2D;
        
        private bool _previouslyLoadingChunks;

        public WorldMapUI(ChunkLoaderEntity chunkLoaderEntity)
        {
            _chunkLoaderEntity = chunkLoaderEntity;
            _mapChunkTexture2D = new MapChunkTexture2D[ChunkLoaderEntity.LoadDistance,ChunkLoaderEntity.LoadDistance];
        }

        public override void LoadContent(ContentManager content) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawMap(spriteBatch, 10, 10);
        }

        private void DrawMap(SpriteBatch spriteBatch, int xOffset, int yOffset)
        {
            for (var x = 0; x < _mapChunkTexture2D.GetLength(0); x++)
            {
                for (var y = 0; y < _mapChunkTexture2D.GetLength(1); y++)
                {
                    if (_mapChunkTexture2D[x,y] != null)
                    {
                        var positionX = (ChunkLoaderEntity.LoadDistance - x) * ChunkLoaderEntity.ChunkSize - ChunkLoaderEntity.ChunkSize + xOffset;
                        var positionY = (ChunkLoaderEntity.LoadDistance - y) * ChunkLoaderEntity.ChunkSize - ChunkLoaderEntity.ChunkSize + yOffset;
                        
                        spriteBatch.Draw(_mapChunkTexture2D[x,y], new Vector2(positionX, positionY), Color.White);
                    }
                }
            }   
        }

        public void LoadTextureChunks()
        {       
            if (!_chunkLoaderEntity.IsLoadingChunks())
            {
                var chunks = _chunkLoaderEntity.GetLoadedChunks();

                for (var x = 0; x < chunks.GetLength(0); x++)
                {
                    for (var y = 0; y < chunks.GetLength(1); y++)
                    {
                        if (chunks[x,y] != null)
                        {
                            _mapChunkTexture2D[x,y] = new MapChunkTexture2D(_graphicsDevice, chunks[x,y].GetChunkData(), 0);
                        }
                    }
                }
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            if(_chunkLoaderEntity.LoadedChunksLastFrame())
                LoadTextureChunks();
        }
    }
}