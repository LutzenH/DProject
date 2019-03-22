using System;
using System.Collections.Generic;
using System.Threading;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type;
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

        private TerrainEntity[,] _loadedChunks = new TerrainEntity[5, 5];

        public const int ChunkSize = 64;

        public enum ChunkLoadingStatus
        {
            Busy,
            Done
        }

        private ChunkLoadingStatus _loadingStatus;

        public ChunkLoaderEntity(EntityManager entityManager) : base(Vector3.Zero, Quaternion.Identity,
            new Vector3(1, 1, 1))
        {
            _entityManager = entityManager;
            _chunkPosition = new Vector2(0, 0);
            _previousChunkPosition = new Vector2(-1, 0);
            _loadingStatus = ChunkLoadingStatus.Done;
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
        }

        public void Update(GameTime gameTime)
        {
            _chunkPosition = CalculateChunkPosition(_entityManager.GetActiveCamera().GetPosition().X,
                _entityManager.GetActiveCamera().GetPosition().Z);

            if (!_chunkPosition.Equals(_previousChunkPosition))
                LoadChunks(_chunkPosition);

            _previousChunkPosition = _chunkPosition;

            foreach (var chunk in _loadedChunks)
                chunk?.UpdateHeightMap();
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
            if (_loadingStatus == ChunkLoadingStatus.Done)
            {
                int oldChunksCount = 0;
                int newChunksCount = 0;

                TerrainEntity[,] oldChunks = _loadedChunks;

                _loadedChunks = new TerrainEntity[_loadedChunks.GetLength(0), _loadedChunks.GetLength(1)];

                List<Vector2> newChunkPositions = new List<Vector2>();
                List<Vector2> newChunkLocations = new List<Vector2>();

                for (int x = 0; x < _loadedChunks.GetLength(0); x++)
                {
                    for (int y = 0; y < _loadedChunks.GetLength(1); y++)
                    {
                        Vector2 position = chunkPosition - new Vector2(x - _loadedChunks.GetLength(0) / 2,
                                               y - _loadedChunks.GetLength(1) / 2);

                        foreach (var chunk in oldChunks)
                        {
                            if (chunk != null)
                            {
                                if (chunk.GetChunkX() == (int) position.X && chunk.GetChunkY() == (int) position.Y)
                                {
                                    _loadedChunks[x, y] = chunk;
                                    oldChunksCount++;
                                }
                            }
                        }

                        if (_loadedChunks[x, y] == null)
                        {
                            newChunksCount++;
                            newChunkPositions.Add(new Vector2(x, y));
                            newChunkLocations.Add(new Vector2(position.X, position.Y));
                        }
                    }
                }

                _entityManager.AddMessage(new Message("Loading new chunks: " + oldChunksCount + " chunks reused and " + newChunksCount + " new chunks."));

                Thread thread = new Thread((() => LoadNewChunks(newChunkPositions, newChunkLocations)));
                thread.Start();
                _loadingStatus = ChunkLoadingStatus.Busy;
            }
        }

        private void LoadNewChunks(List<Vector2> newChunkPositions, List<Vector2> newChunkLocations)
        {
            for (int i = 0; i < newChunkPositions.Count; i++)
            {
                _loadedChunks[(int) newChunkPositions[i].X, (int) newChunkPositions[i].Y] =
                    LoadChunk((int) newChunkLocations[i].X, (int) newChunkLocations[i].Y);
            }

            _entityManager.AddMessage(new Message("Done loading new chunks."));
            _loadingStatus = ChunkLoadingStatus.Done;
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
                (int) (x - (ChunkSize / 2)) / ChunkSize,
                (int) (y - (ChunkSize / 2)) / ChunkSize);
        }

        public float? GetHeightFromPosition(Vector2 position, int floor)
        {
            Vector2 tempPosition =
                new Vector2((float) Math.Floor(position.X + 0.5f), (float) Math.Floor(position.Y + 0.5f));

            int chunkPositionX = (int) Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int) Math.Floor(tempPosition.Y / ChunkSize);

            int localChunkPositionX = (int) tempPosition.X - (chunkPositionX * ChunkSize);
            int localChunkPositionY = (int) tempPosition.Y - (chunkPositionY * ChunkSize);

            if (localChunkPositionX < 0)
                localChunkPositionX = ChunkSize + localChunkPositionX;

            if (localChunkPositionY < 0)
                localChunkPositionY = ChunkSize + localChunkPositionY;

            foreach (var chunk in _loadedChunks)
            {
                if (chunk != null)
                {
                    if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                        return chunk.GetTileHeight(localChunkPositionX, localChunkPositionY, floor);
                }
            }

            return null;
        }

        public byte? GetVertexHeight(Vector2 position, TerrainEntity.TileCorner corner, int floor)
        {
            Vector2 tempPosition =
                new Vector2((float) Math.Floor(position.X + 0.5f), (float) Math.Floor(position.Y + 0.5f));

            int chunkPositionX = (int) Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int) Math.Floor(tempPosition.Y / ChunkSize);

            int localChunkPositionX = (int) tempPosition.X - (chunkPositionX * ChunkSize);
            int localChunkPositionY = (int) tempPosition.Y - (chunkPositionY * ChunkSize);

            if (localChunkPositionX < 0)
                localChunkPositionX = ChunkSize + localChunkPositionX;

            if (localChunkPositionY < 0)
                localChunkPositionY = ChunkSize + localChunkPositionY;

            foreach (var chunk in _loadedChunks)
            {
                if (chunk != null)
                {
                    if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                        return chunk.GetVertexHeight(localChunkPositionX, localChunkPositionY, floor, corner);
                }
            }

            return null;
        }

        public static Vector2 GetLocalChunkPosition(Vector2 position)
        {
            Vector2 tempPosition =
                new Vector2((float) Math.Floor(position.X + 0.5f), (float) Math.Floor(position.Y + 0.5f));

            int chunkPositionX = (int) Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int) Math.Floor(tempPosition.Y / ChunkSize);

            int localChunkPositionX = (int) tempPosition.X - (chunkPositionX * ChunkSize);
            int localChunkPositionY = (int) tempPosition.Y - (chunkPositionY * ChunkSize);

            return new Vector2(localChunkPositionX, localChunkPositionY);
        }

        public TerrainEntity GetChunk(Vector3 position)
        {
            Vector2 tempPosition =
                new Vector2((float) Math.Floor(position.X + 0.5f), (float) Math.Floor(position.Z + 0.5f));

            int chunkPositionX = (int) Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int) Math.Floor(tempPosition.Y / ChunkSize);

            foreach (var chunk in _loadedChunks)
            {
                if (chunk != null)
                {
                    if (chunk.GetChunkX() == chunkPositionX && chunk.GetChunkY() == chunkPositionY)
                    {
                        return chunk;
                    }
                }
            }

            return null;
        }

        public TerrainEntity GetChunk(int chunkX, int chunkY)
        {
            foreach (var chunk in _loadedChunks)
            {
                if (chunk.GetChunkX() == chunkX && chunk.GetChunkY() == chunkY)
                    return chunk;
            }

            return null;
        }

        public void ChangeTileHeight(byte height, Vector3 position, int floor, int brushSize)
        {
            if (brushSize > 0)
            {
                ChangeCornerHeight(height, position, floor, TerrainEntity.TileCorner.TopLeft);
                ChangeCornerHeight(height, position, floor, TerrainEntity.TileCorner.TopRight);
                ChangeCornerHeight(height, position, floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerHeight(height, position, floor, TerrainEntity.TileCorner.BottomRight);
            }

            if (brushSize == 2)
            {
                ChangeCornerHeight(height, new Vector3(position.X + 1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                ChangeCornerHeight(height, new Vector3(position.X + 1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                
                ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerHeight(height, new Vector3(position.X + 1, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerHeight(height, new Vector3(position.X + 1, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomRight);
            }
            else
            {
                if (brushSize > 2)
                {
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 3)
                {
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z-1), floor,  TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 4)
                {
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                    
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z+3), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z+3), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z-3), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X, position.Y, position.Z-3), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 5)
                {
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopLeft);
                    
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopRight);
       
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X-1, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomLeft);
                    
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                    ChangeCornerHeight(height, new Vector3(position.X+1, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 6)
                {
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerHeight(height, new Vector3(position.X-2, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerHeight(height, new Vector3(position.X+2, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomRight);
                }
            }
        }

        public void ChangeTileTexture(ushort? textureId, Vector3 position, int floor, int brushSize, bool alternativeTriangle)
        {
            if (brushSize == 0)
            {
                var localChunkPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));
                var x = (int)localChunkPosition.X;
                var y = (int) localChunkPosition.Y;
            
                GetChunk(position).ChangeTriangleTexture(textureId, x, y, floor, alternativeTriangle);
            }
            else
            {
                ChangeTileTexture(textureId, position, floor);
                if (brushSize == 2)
                {
                    ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z), floor);
                    ChangeTileTexture(textureId, new Vector3(position.X, position.Y, position.Z+1), floor);
                    ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z+1), floor);
                }
                else
                {
                    if (brushSize > 2)
                    {
                        ChangeTileTexture(textureId, new Vector3(position.X-1, position.Y, position.Z), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X, position.Y, position.Z-1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X, position.Y, position.Z+1), floor);
                    }
                    if (brushSize > 3)
                    {
                        ChangeTileTexture(textureId, new Vector3(position.X-1, position.Y, position.Z-1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z-1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X-1, position.Y, position.Z+1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z+1), floor);
                    }
                    if (brushSize > 4)
                    {
                        ChangeTileTexture(textureId, new Vector3(position.X-2, position.Y, position.Z), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+2, position.Y, position.Z), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X, position.Y, position.Z+2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X, position.Y, position.Z-2), floor);
                    }
                    if (brushSize > 5)
                    {
                        ChangeTileTexture(textureId, new Vector3(position.X-2, position.Y, position.Z-1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X-2, position.Y, position.Z+1), floor);
    
                        ChangeTileTexture(textureId, new Vector3(position.X+2, position.Y, position.Z-1), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+2, position.Y, position.Z+1), floor);
                        
                        ChangeTileTexture(textureId, new Vector3(position.X-1, position.Y, position.Z-2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z-2), floor);
                        
                        ChangeTileTexture(textureId, new Vector3(position.X-1, position.Y, position.Z+2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+1, position.Y, position.Z+2), floor);
                    }
                    if (brushSize > 6)
                    {
                        ChangeTileTexture(textureId, new Vector3(position.X-2, position.Y, position.Z-2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X-2, position.Y, position.Z+2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+2, position.Y, position.Z-2), floor);
                        ChangeTileTexture(textureId, new Vector3(position.X+2, position.Y, position.Z+2), floor);
                    }
                }
            }
        }

        public void ChangeTileTexture(ushort? textureId, Vector3 position, int floor)
        {
            Vector2 localChunkPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));

            int x = (int)localChunkPosition.X;
            int y = (int) localChunkPosition.Y;
            
            GetChunk(position).ChangeTileTexture(textureId, x, y, floor);
        }

        public void ChangeCornerHeight(byte height, Vector3 position, int floor, TerrainEntity.TileCorner tileCorner)
        {            
            var localPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            Vector2 tempPosition = new Vector2((float)Math.Floor(position.X + 0.5f), (float)Math.Floor(position.Z+0.5f));
            
            int chunkPositionX = (int)Math.Floor(tempPosition.X / ChunkSize);
            int chunkPositionY = (int)Math.Floor(tempPosition.Y / ChunkSize);
            
            switch (tileCorner)
            {
                case TerrainEntity.TileCorner.TopLeft:
                {
                    if ((int) localPosition.X == 0 && (int) localPosition.Y == 0)
                        GetChunk(chunkPositionX-1, chunkPositionY-1).ChangeVertexHeight(height, ChunkSize-1, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomRight);
                
                    if ((int) localPosition.X == 0)
                        GetChunk(chunkPositionX-1, chunkPositionY).ChangeVertexHeight(height, ChunkSize-1, (int)localPosition.Y, floor, TerrainEntity.TileCorner.TopRight);

                    if ((int) localPosition.Y == 0)
                        GetChunk(chunkPositionX, chunkPositionY-1).ChangeVertexHeight(height, (int)localPosition.X, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomLeft);
                    break;
                }
                
                case TerrainEntity.TileCorner.TopRight:
                {
                    if ((int) localPosition.X == ChunkSize-1 && (int) localPosition.Y == 0)
                        GetChunk(chunkPositionX+1, chunkPositionY-1).ChangeVertexHeight(height, 0, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomLeft);
                
                    if ((int) localPosition.X == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY).ChangeVertexHeight(height, 0, (int)localPosition.Y, floor, TerrainEntity.TileCorner.TopLeft);
                
                    if ((int) localPosition.Y == 0)
                        GetChunk(chunkPositionX, chunkPositionY-1).ChangeVertexHeight(height, (int)localPosition.X, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomRight);
                    break;
                }

                case TerrainEntity.TileCorner.BottomLeft:
                {
                    if ((int) localPosition.X == 0 && (int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX-1, chunkPositionY+1).ChangeVertexHeight(height, ChunkSize-1, 0, floor, TerrainEntity.TileCorner.TopRight);
                    
                    if((int) localPosition.X == 0)
                        GetChunk(chunkPositionX-1, chunkPositionY).ChangeVertexHeight(height, ChunkSize-1, (int)localPosition.Y, floor, TerrainEntity.TileCorner.BottomRight);
                    
                    if((int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX, chunkPositionY+1).ChangeVertexHeight(height, (int)localPosition.X, 0, floor, TerrainEntity.TileCorner.TopLeft);
                    
                    break;
                }

                case TerrainEntity.TileCorner.BottomRight:
                {
                    if ((int) localPosition.X == ChunkSize-1 && (int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY+1).ChangeVertexHeight(height, 0, 0, floor, TerrainEntity.TileCorner.TopLeft);
                    
                    if ((int) localPosition.X == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY).ChangeVertexHeight(height, 0, (int)localPosition.Y, floor, TerrainEntity.TileCorner.BottomLeft);
                    
                    if((int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX, chunkPositionY+1).ChangeVertexHeight(height, (int)localPosition.X, 0, floor, TerrainEntity.TileCorner.TopRight);
                    
                    break;
                }
            }

            GetChunk(position).ChangeVertexHeight(height, (int)localPosition.X, (int)localPosition.Y, floor, tileCorner);
        }

        public void PlaceProp(Vector3 position, int floor, ushort objectId)
        {
            var localPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            GetChunk(position).PlaceProp((int)localPosition.X, (int)localPosition.Y, floor, objectId);
        }

        public void RemoveProp(Vector3 position, int floor)
        {
            var localPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));            
            
            GetChunk(position).RemoveProp((int)localPosition.X, (int)localPosition.Y, floor);
        }

        public TerrainEntity[,] GetLoadedChunks()
        {
            return _loadedChunks;
        }

        public bool IsLoadingChunks()
        {
            return (_loadingStatus == ChunkLoadingStatus.Busy);
        }
    }
}