using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if EDITOR
using Gtk;
#else
using System.Threading.Tasks;
using System.Threading;
#endif
using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type;
using DProject.Type.Enum;
using DProject.Type.Rendering;
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
        
        private ConcurrentDictionary<(int, int), TerrainEntity> _loadedChunks;
        
        private bool _loadedChunksLastFrame;
        
#if !EDITOR
        private CancellationTokenSource _cancellationToken;
#endif
        public enum ChunkLoadingStatus
        {
            Busy,
            Done
        }

        private ChunkLoadingStatus _loadingStatus;

        public ChunkLoaderEntity(EntityManager entityManager) : base(entityManager, Vector3.Zero, Quaternion.Identity,
            new Vector3(1, 1, 1))
        {
            _loadedChunks = new ConcurrentDictionary<(int, int), TerrainEntity>();
            
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
            _chunkPosition = CalculateChunkPosition(EntityManager.GetActiveCamera().GetPosition());

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
            {
                if(_loadedChunks.ContainsKey(chunk.Key))
                    chunk.Value.UpdateHeightMap();
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            foreach (var chunk in _loadedChunks)
            {
                if(_loadedChunks.ContainsKey(chunk.Key))
                    chunk.Value.Draw(activeCamera);
            }
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
                var newChunkPositions = new List<(int, int, LevelOfDetail)>();
                
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
                            var levelOfDetail = LevelOfDetail.Full;

                            newChunksCount++;
                            newChunkPositions.Add((position.Item1, position.Item2, levelOfDetail));
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

                _loadingStatus = ChunkLoadingStatus.Busy;

                var deadChunks = _loadedChunks.Keys.Except(oldChunkPositions).ToArray();

                foreach (var chunk in deadChunks)
                {
                    if (!_loadedChunks.TryRemove(chunk, out var terrainEntity))
                        Console.WriteLine("Failed to remove: " + terrainEntity.GetChunkX() + ", " + terrainEntity.GetChunkY());
                }

                EditorEntityManager.AddMessage(new Message("Loading new chunks: " + oldChunksCount + " chunks reused and " + newChunksCount + " new chunks."));
                
                LoadNewChunks(newChunkPositions);

                _loadedChunksLastFrame = true;
            }
        }

        private void LoadNewChunks(List<(int,int, LevelOfDetail)> newChunkPositions)
        {
#if EDITOR
            Application.Invoke((sender, args) =>
            {
                foreach (var newChunk in newChunkPositions)
                {
                    var pos = (newChunk.Item1, newChunk.Item2);
                    var lod = newChunk.Item3;
            
                    _loadedChunks[pos] = LoadChunk(pos, lod);
                }
            });
            
            EditorEntityManager.AddMessage(new Message("Done loading new chunks."));
            _loadingStatus = ChunkLoadingStatus.Done;
#else
            AbortLoadingChunks();

            _cancellationToken = new CancellationTokenSource();

            var tasks = new Task<TerrainEntity>[newChunkPositions.Count];

            for (var i = 0; i < newChunkPositions.Count; i++)
            {
                var pos = (newChunkPositions[i].Item1, newChunkPositions[i].Item2);
                var lod = newChunkPositions[i].Item3;
            
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (_cancellationToken.Token.Register(AbortLoadingChunks))
                        {
                            return _loadedChunks[pos] = LoadChunk(pos, lod);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }

            if (newChunkPositions.Count != 0)
            {
                Task.Factory.ContinueWhenAll(tasks, tasks1 =>
                {
                    EditorEntityManager.AddMessage(new Message("Done loading new chunks."));
                    _loadingStatus = ChunkLoadingStatus.Done;
                });
            }
#endif

            if (_loadedChunks.ContainsKey((_chunkPosition.Item1, _chunkPosition.Item2)))
            {
                LightingProperties.CurrentInfo = _loadedChunks[(_chunkPosition.Item1, _chunkPosition.Item2)].GetChunkData().LightingInfo;
                HeightMap.TerrainEffect.SetLightingInfo(LightingProperties.CurrentInfo);
            }
        }

#if !EDITOR
        private void AbortLoadingChunks()
        {
            _cancellationToken?.Cancel();
        }
#endif
        
        private TerrainEntity LoadChunk((int, int) position, LevelOfDetail levelOfDetail)
        {
            var chunk = new TerrainEntity(position.Item1, position.Item2, levelOfDetail);
            
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

                var positions = _loadedChunks.Keys.ToArray();
                
                foreach (var key in positions)
                {
                    if (_loadedChunks[key].GetChunkData().ChunkStatus == ChunkStatus.Changed)
                    {
                        _loadedChunks[key] = LoadChunk(key, _loadedChunks[key].GetHeightMap().GetLevelOfDetail());
                        count++;
                    }
                }

                EditorEntityManager.AddMessage(new Message("Reloaded " + count + " changed chunks."));
            }
        }

        #endregion
        
        #region Chunk Information
        
        public static (int, int) CalculateChunkPosition(Vector3 position)
        {
            var xPos = Math.Floor(position.X + 0.5f);
            var yPos = Math.Floor(position.Z + 0.5f);

            var chunkPositionX = (int) Math.Floor(xPos / ChunkSize);
            var chunkPositionY = (int) Math.Floor(yPos / ChunkSize);

            return (chunkPositionX, chunkPositionY);
        }

        public static (int, int) CalculateLocalChunkPosition((int, int) position)
        {
            var (x, y) = position;
            
            var chunkPositionX = x / ChunkSize;
            var chunkPositionY = y / ChunkSize;
            
            var localChunkPositionX = x - chunkPositionX * ChunkSize;
            var localChunkPositionY = y - chunkPositionY * ChunkSize;

            return (localChunkPositionX, localChunkPositionY);
        }

        public TerrainEntity GetChunk(Vector3 position)
        {
            var chunkPosition = CalculateChunkPosition(position);
            
            return GetChunk(chunkPosition);
        }
        
        public TerrainEntity GetChunk((int, int) chunkPosition)
        {
            return _loadedChunks.ContainsKey(chunkPosition) ? _loadedChunks[chunkPosition] : null;
        }
        
        public IEnumerator<KeyValuePair<(int, int), TerrainEntity>> GetLoadedChunks()
        {
            return _loadedChunks.GetEnumerator();
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

        public ushort? GetVertexHeight(Vector2 position)
        {
            var (x, y) = new Vector2((float) Math.Floor(position.X + 0.5f), (float) Math.Floor(position.Y + 0.5f));

            var chunkPositionX = (int) Math.Floor(x / ChunkSize);
            var chunkPositionY = (int) Math.Floor(y / ChunkSize);

            var localChunkPositionX = (int) x - (chunkPositionX * ChunkSize);
            var localChunkPositionY = (int) y - (chunkPositionY * ChunkSize);

            if (localChunkPositionX < 0)
                localChunkPositionX = ChunkSize + localChunkPositionX;

            if (localChunkPositionY < 0)
                localChunkPositionY = ChunkSize + localChunkPositionY;

            return _loadedChunks[(chunkPositionX, chunkPositionY)].GetVertexHeight(localChunkPositionX, localChunkPositionY);
        }

        public void ChangeVertexTexture(ushort? textureId, Vector3 position)
        {
            var localChunkPosition = CalculateLocalChunkPosition(((int, int)) (position.X, position.Z));

            GetChunk(position).ChangeVertexTexture(textureId, localChunkPosition.Item1, localChunkPosition.Item2);
        }

        public void ChangeVertexColor(ushort color, Vector3 position)
        {
            var localChunkPosition = CalculateLocalChunkPosition(((int, int)) (position.X, position.Z));

            GetChunk(position).ChangeVertexColor(color, localChunkPosition.Item1, localChunkPosition.Item2);
        }

        public void ChangeVertexHeight(ushort height, Vector3 position)
        {            
            var localChunkPosition = CalculateLocalChunkPosition(((int, int)) (position.X, position.Z));

            GetChunk(position).ChangeVertexHeight(height, localChunkPosition.Item1, localChunkPosition.Item2);
        }

        public void PlaceProp(Vector3 position, Rotation rotation, ushort objectId)
        {
            var (x, y) = CalculateLocalChunkPosition(((int, int)) (position.X, position.Z));

            GetChunk(position).PlaceProp((int)x, (int)y, rotation, objectId);
        }

        public void RemoveProp(Vector3 position)
        {
            var (x, y) = CalculateLocalChunkPosition(((int, int)) (position.X, position.Z));

            GetChunk(position).RemoveProp((int)x, (int)y);
        }
        
        #endregion
    }
}