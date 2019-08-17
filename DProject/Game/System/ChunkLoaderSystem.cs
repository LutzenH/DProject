using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DProject.Game;
using DProject.List;
using DProject.Type.Enum;
using DProject.Type.Rendering;
using DProject.Type.Serializable.Chunk;
using MessagePack;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Object = System.Object;

namespace DProject.Manager.System
{
    public class ChunkLoaderSystem : UpdateSystem
    {
        private const int DefaultLoadDistance = 8;
        private const int ChunkSize = 64;
        
        private readonly EntityFactory _entityFactory;
        private readonly ConcurrentDictionary<(int, int), Entity> _loadedChunks;

        private (int, int) _previousChunkPosition;
        private (int, int) _chunkPosition;

        private readonly int _loadDistance;
        
        public ChunkLoaderSystem(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
            
            _loadedChunks = new ConcurrentDictionary<(int, int), Entity>();
            
            _loadDistance = DefaultLoadDistance;
            
            _chunkPosition = (0, 0);
            _previousChunkPosition = (-1, 0);
        }

        public override void Update(GameTime gameTime)
        {
            _chunkPosition = CalculateChunkPosition(CameraSystem.ActiveLens.Position);
            
            if (!_chunkPosition.Equals(_previousChunkPosition))
                LoadChunks(_chunkPosition);

            _previousChunkPosition = _chunkPosition;
        }
        
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
        
        #endregion

        #region Saving and loading

        private void LoadChunks((int, int) chunkPosition)
        {
            var oldChunkPositions = new List<(int, int)>();
            var newChunkPositions = new List<(int, int)>();
            
            int x, y, dx, dy;
            x = y = dx =0;
            dy = -1;
            var t = Math.Max(_loadDistance, _loadDistance);
            var maxI = t * t;
            for(var i = 0; i < maxI; i++){
                if (-_loadDistance/2 <= x 
                    && x <= _loadDistance/2 
                    && -_loadDistance/2 <= y 
                    && y <= _loadDistance/2)
                {
                    var position = (chunkPosition.Item1 + x, chunkPosition.Item2 + y);

                    if (_loadedChunks.ContainsKey(position))
                        oldChunkPositions.Add(position);
                    else
                        newChunkPositions.Add((position.Item1, position.Item2));
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
            {
                _loadedChunks.TryRemove(chunk, out var entity);
                entity.Destroy();
            }

            foreach (var chunk in newChunkPositions)
            {
                var xPos = chunk.Item1 * ChunkSize;
                var yPos = chunk.Item2 * ChunkSize;
                _loadedChunks[chunk] = _entityFactory.CreateHeightmap(new Vector3(xPos, 0, yPos), GenerateChunkData(chunk.Item1, chunk.Item2).VertexMap);
            }

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
                var shortMap = Noise.GenerateNoiseMap(ChunkSize, ChunkSize, x * ChunkSize, y * ChunkSize, 50f);
                var vertices = HeightmapLoaderSystem.GenerateVertexMap(shortMap);
                
                chunkData = new ChunkData()
                {
                    ChunkPositionX = x,
                    ChunkPositionY = y,
                    VertexMap = vertices,
                    
                    Objects = new List<Type.Serializable.Chunk.Object>(),
                    
                    SkyId = Skies.GetDefaultSkyId(),
                    
                    ChunkStatus = ChunkStatus.Unserialized
                };
            }

            return chunkData;
        }

        #endregion
    }
}
