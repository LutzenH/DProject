using System;
using System.Collections.Generic;
using System.Linq;

#if EDITOR
using Gtk;
#else
using System.Threading;
#endif

using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Chunk
{
    public class ChunkLoaderEntity : AbstractAwareEntity, IDrawable, IInitialize, IUpdateable
    {
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;

        private (int, int) _previousChunkPosition;
        private (int, int) _chunkPosition;

        public const int LoadDistance = 8;
        public const int ChunkSize = 64;
        
        private Dictionary<(int, int), TerrainEntity> _loadedChunks;
        
        private bool _loadedChunksLastFrame;
        
        public enum ChunkLoadingStatus
        {
            Busy,
            Done
        }

        private ChunkLoadingStatus _loadingStatus;

        public ChunkLoaderEntity(EntityManager entityManager) : base(entityManager, Vector3.Zero, Quaternion.Identity,
            new Vector3(1, 1, 1))
        {
            _loadedChunks = new Dictionary<(int, int), TerrainEntity>();
            
            _chunkPosition = (0, 0);
            _previousChunkPosition = (-1, 0);
            _loadingStatus = ChunkLoadingStatus.Done;
        }

        public override void LoadContent(ContentManager content)
        {
            _contentManager = content;
        }

        public void Update(GameTime gameTime)
        {
            _chunkPosition = CalculateChunkPosition(EntityManager.GetActiveCamera().GetPosition().X,
                EntityManager.GetActiveCamera().GetPosition().Z);

            if (!_chunkPosition.Equals(_previousChunkPosition))
            {
                LoadChunks(_chunkPosition);
            }
            else
            {
                if (_loadedChunksLastFrame)
                    _loadedChunksLastFrame = false;
            }

            _previousChunkPosition = _chunkPosition;

            foreach (var chunk in _loadedChunks)
                chunk.Value.UpdateHeightMap();
        }

        public void Draw(CameraEntity activeCamera)
        {
            foreach (var chunk in _loadedChunks)
                chunk.Value.Draw(activeCamera);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        #region Saving and loading
        
        private void LoadChunks((int, int) chunkPosition)
        {
            if (_loadingStatus == ChunkLoadingStatus.Done)
            {
                var oldChunksCount = 0;
                var newChunksCount = 0;
                
                var oldChunkPositions = new List<(int, int)>();
                var newChunkPositions = new List<(int, int)>();
                
                int x, y, dx, dy;
                x = y = dx =0;
                dy = -1;
                var t = Math.Max(LoadDistance, LoadDistance);
                var maxI = t * t;
                for(var i = 0; i < maxI; i++){
                    if (-LoadDistance/2 <= x 
                        && x <= LoadDistance/2 
                        && -LoadDistance/2 <= y 
                        && y <= LoadDistance/2)
                    {
                        var position = (chunkPosition.Item1 + x, chunkPosition.Item2 + y);

                        if (_loadedChunks.ContainsKey(position))
                        {
                            oldChunksCount++;
                            oldChunkPositions.Add(position);
                        }
                        else
                        {
                            newChunksCount++;
                            newChunkPositions.Add(position);
                        }
                    }
                    if(x == y || x < 0 && x == -y || x > 0 && x == 1-y)
                    {
                        t = dx;
                        dx = -dy;
                        dy = t;
                    }
                    x += dx;
                    y += dy;
                }

                var deadChunks = _loadedChunks.Keys.Except(oldChunkPositions).ToArray();
                
                foreach (var chunk in deadChunks)
                    _loadedChunks.Remove(chunk);
                
                EditorEntityManager.AddMessage(new Message("Loading new chunks: " + oldChunksCount + " chunks reused and " + newChunksCount + " new chunks."));

                _loadingStatus = ChunkLoadingStatus.Busy;

#if EDITOR
                Application.Invoke((sender, args) => LoadNewChunks(newChunkPositions));           
#else
                //Use this instead of Application.Invoke when not using the GTK editor
                Thread thread = new Thread((() => LoadNewChunks(newChunkPositions)));
                thread.Start();
#endif
                
                _loadedChunksLastFrame = true;
            }
        }

        private void LoadNewChunks(List<(int,int)> newChunkPositions)
        {            
            foreach (var position in newChunkPositions)
                _loadedChunks[position] = LoadChunk(position);

            EditorEntityManager.AddMessage(new Message("Done loading new chunks."));
            _loadingStatus = ChunkLoadingStatus.Done;
        }

        private TerrainEntity LoadChunk((int, int) position)
        {
            var chunk = new TerrainEntity(position.Item1, position.Item2);
            
            chunk.Initialize(_graphicsDevice);
            chunk.LoadContent(_contentManager);

            return chunk;
        }

        public void SerializeChangedChunks()
        {
            if (_loadingStatus == ChunkLoadingStatus.Done)
            {
                EditorEntityManager.AddMessage(new Message("Starting serialization of changed chunks.."));

                int count = 0;
                
                foreach (var key in _loadedChunks.Keys)
                {
                    if (_loadedChunks[key].GetChunkData().ChunkStatus == ChunkStatus.Changed)
                    {
                        _loadedChunks[key].Serialize();
                        count++;
                    }
                }
                
                EditorEntityManager.AddMessage(new Message("Serialized " + count + " changed chunks."));
            }
        }

        public void ReloadChangedChunks()
        {
            if (_loadingStatus == ChunkLoadingStatus.Done)
            {
                EditorEntityManager.AddMessage(new Message("Reloading changed chunks.."));

                var count = 0;

                foreach (var key in _loadedChunks.Keys)
                {
                    if (_loadedChunks[key].GetChunkData().ChunkStatus == ChunkStatus.Changed)
                    {
                        _loadedChunks[key] = LoadChunk(key);
                        count++;
                    }
                }

                EditorEntityManager.AddMessage(new Message("Reloaded " + count + " changed chunks."));
            }
        }

        #endregion
        
        #region Chunk Information
        
        public static (int, int) CalculateChunkPosition(float x, float y)
        {
            return ((int) (x - (ChunkSize / 2)) / ChunkSize, (int) (y - (ChunkSize / 2)) / ChunkSize);
        }
        
        public static Vector2 GetLocalChunkPosition(Vector2 position)
        {
            var x = Math.Floor(position.X + 0.5f);
            var y = Math.Floor(position.Y + 0.5f);

            var chunkPositionX = (int) Math.Floor(x / ChunkSize);
            var chunkPositionY = (int) Math.Floor(y / ChunkSize);

            var localChunkPositionX = (int) x - chunkPositionX * ChunkSize;
            var localChunkPositionY = (int) y - chunkPositionY * ChunkSize;

            return new Vector2(localChunkPositionX, localChunkPositionY);
        }

        public TerrainEntity GetChunk(Vector3 position)
        {
            var x = Math.Floor(position.X + 0.5f);
            var y = Math.Floor(position.Z + 0.5f);

            var chunkPositionX = (int) Math.Floor(x / ChunkSize);
            var chunkPositionY = (int) Math.Floor(y / ChunkSize);

            return GetChunk(chunkPositionX, chunkPositionY);
        }
        
        public TerrainEntity GetChunk(int chunkX, int chunkY)
        {
            if (_loadedChunks.ContainsKey((chunkX, chunkY)))
                return _loadedChunks[(chunkX, chunkY)];
            else
                return null;
        }
        
        public Dictionary<(int, int), TerrainEntity> GetLoadedChunks()
        {
            return _loadedChunks;
        }

        public bool IsLoadingChunks()
        {
            return (_loadingStatus == ChunkLoadingStatus.Busy);
        }

        public bool GetLoadedChunksLastFrame()
        {
            return _loadedChunksLastFrame;
        }

        #endregion

        #region Chunk Editing
        
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

            return _loadedChunks[(chunkPositionX, chunkPositionY)].GetTileHeight(localChunkPositionX, localChunkPositionY, floor);
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

            return _loadedChunks[(chunkPositionX, chunkPositionY)].GetVertexHeight(localChunkPositionX, localChunkPositionY, floor, corner);
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
        
        public void ChangeTileColor(ushort color, Vector3 position, int floor, int brushSize)
        {
            if (brushSize > 0)
            {
                ChangeCornerColor(color, position, floor, TerrainEntity.TileCorner.TopLeft);
                ChangeCornerColor(color, position, floor, TerrainEntity.TileCorner.TopRight);
                ChangeCornerColor(color, position, floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerColor(color, position, floor, TerrainEntity.TileCorner.BottomRight);
            }

            if (brushSize == 2)
            {
                ChangeCornerColor(color, new Vector3(position.X + 1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                ChangeCornerColor(color, new Vector3(position.X + 1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                
                ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerColor(color, new Vector3(position.X + 1, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomLeft);
                ChangeCornerColor(color, new Vector3(position.X + 1, position.Y, position.Z + 1), floor, TerrainEntity.TileCorner.BottomRight);
            }
            else
            {
                if (brushSize > 2)
                {
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 3)
                {
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z-1), floor,  TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 4)
                {
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z), floor, TerrainEntity.TileCorner.BottomRight);
                    
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z+3), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z+3), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z-3), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X, position.Y, position.Z-3), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 5)
                {
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopLeft);
                    
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z-1), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopRight);
       
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X-1, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomLeft);
                    
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z+1), floor, TerrainEntity.TileCorner.BottomRight);
                    ChangeCornerColor(color, new Vector3(position.X+1, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomRight);
                }
                if (brushSize > 6)
                {
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopLeft);
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z-2), floor, TerrainEntity.TileCorner.TopRight);
                    ChangeCornerColor(color, new Vector3(position.X-2, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomLeft);
                    ChangeCornerColor(color, new Vector3(position.X+2, position.Y, position.Z+2), floor, TerrainEntity.TileCorner.BottomRight);
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

        public void ChangeCornerColor(ushort color, Vector3 position, int floor, TerrainEntity.TileCorner tileCorner)
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
                        GetChunk(chunkPositionX-1, chunkPositionY-1).ChangeVertexColor(color, ChunkSize-1, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomRight);
                
                    if ((int) localPosition.X == 0)
                        GetChunk(chunkPositionX-1, chunkPositionY).ChangeVertexColor(color, ChunkSize-1, (int)localPosition.Y, floor, TerrainEntity.TileCorner.TopRight);

                    if ((int) localPosition.Y == 0)
                        GetChunk(chunkPositionX, chunkPositionY-1).ChangeVertexColor(color, (int)localPosition.X, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomLeft);
                    break;
                }
                
                case TerrainEntity.TileCorner.TopRight:
                {
                    if ((int) localPosition.X == ChunkSize-1 && (int) localPosition.Y == 0)
                        GetChunk(chunkPositionX+1, chunkPositionY-1).ChangeVertexColor(color, 0, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomLeft);
                
                    if ((int) localPosition.X == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY).ChangeVertexColor(color, 0, (int)localPosition.Y, floor, TerrainEntity.TileCorner.TopLeft);
                
                    if ((int) localPosition.Y == 0)
                        GetChunk(chunkPositionX, chunkPositionY-1).ChangeVertexColor(color, (int)localPosition.X, ChunkSize-1, floor, TerrainEntity.TileCorner.BottomRight);
                    break;
                }

                case TerrainEntity.TileCorner.BottomLeft:
                {
                    if ((int) localPosition.X == 0 && (int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX-1, chunkPositionY+1).ChangeVertexColor(color, ChunkSize-1, 0, floor, TerrainEntity.TileCorner.TopRight);
                    
                    if((int) localPosition.X == 0)
                        GetChunk(chunkPositionX-1, chunkPositionY).ChangeVertexColor(color, ChunkSize-1, (int)localPosition.Y, floor, TerrainEntity.TileCorner.BottomRight);
                    
                    if((int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX, chunkPositionY+1).ChangeVertexColor(color, (int)localPosition.X, 0, floor, TerrainEntity.TileCorner.TopLeft);
                    
                    break;
                }

                case TerrainEntity.TileCorner.BottomRight:
                {
                    if ((int) localPosition.X == ChunkSize-1 && (int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY+1).ChangeVertexColor(color, 0, 0, floor, TerrainEntity.TileCorner.TopLeft);
                    
                    if ((int) localPosition.X == ChunkSize-1)
                        GetChunk(chunkPositionX+1, chunkPositionY).ChangeVertexColor(color, 0, (int)localPosition.Y, floor, TerrainEntity.TileCorner.BottomLeft);
                    
                    if((int) localPosition.Y == ChunkSize-1)
                        GetChunk(chunkPositionX, chunkPositionY+1).ChangeVertexColor(color, (int)localPosition.X, 0, floor, TerrainEntity.TileCorner.TopRight);
                    
                    break;
                }
            }

            GetChunk(position).ChangeVertexColor(color, (int)localPosition.X, (int)localPosition.Y, floor, tileCorner);
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

        public void PlaceProp(Vector3 position, int floor, Rotation rotation, ushort objectId)
        {
            var localPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            GetChunk(position).PlaceProp((int)localPosition.X, (int)localPosition.Y, floor, rotation, objectId);
        }

        public void RemoveProp(Vector3 position, int floor)
        {
            var localPosition = GetLocalChunkPosition(new Vector2(position.X, position.Z));            
            
            GetChunk(position).RemoveProp((int)localPosition.X, (int)localPosition.Y, floor);
        }
        
        #endregion
    }
}