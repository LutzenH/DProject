using System;
using System.Collections.Generic;
#if EDITOR
using Gtk;
#else
using System.Threading;
#endif
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type.Rendering.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class WorldMapUI : AbstractUI, IInitialize, IUpdateable
    {
        private GraphicsDevice _graphicsDevice;

        private MapChunkTexture2D[,] _mapChunkTexture2D;

        private GameEntityManager _gameEntityManager;
        
        private (int, int) _chunkPosition;
        private (int, int) _positionOffset;

        private readonly Point _position;

        private bool _isLoading;

        private readonly int _mapSize;

        public WorldMapUI(GameEntityManager gameEntityManager, int mapSize, Point position) : base(gameEntityManager)
        {
            _gameEntityManager = gameEntityManager;

            _positionOffset = (0, 0);

            _position = position;
            
            _mapSize = mapSize;
            _mapChunkTexture2D = new MapChunkTexture2D[_mapSize, _mapSize];
        }

        public override void LoadContent(ContentManager content) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawMap(spriteBatch, _position.X, _position.Y);
        }

        private void DrawMap(SpriteBatch spriteBatch, int xOffset, int yOffset)
        {
            for (var x = 0; x < _mapChunkTexture2D.GetLength(0); x++)
            {
                for (var y = 0; y < _mapChunkTexture2D.GetLength(1); y++)
                {
                    if (_mapChunkTexture2D[x,y] != null)
                    {
                        var positionX = x * ChunkLoaderEntity.ChunkSize + xOffset;
                        var positionY = y * ChunkLoaderEntity.ChunkSize + yOffset;
                        
                        spriteBatch.Draw(_mapChunkTexture2D[x,y], new Rectangle(positionX, positionY, ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize), Color.White);
                    }
                }
            }
        }

        public void LoadTextureChunks()
        {
            _isLoading = true;
            
            if (!_gameEntityManager.GetChunkLoaderEntity().IsLoadingChunks())
            {
                MapChunkTexture2D[,] oldChunks = _mapChunkTexture2D;

                _mapChunkTexture2D = new MapChunkTexture2D[_mapSize, _mapSize];
                
                List<(int, int)> newChunkPositions = new List<(int, int)>();
                List<(int, int)> newChunkLocations = new List<(int, int)>();
                
                int x,y,dx,dy;
                x = y = dx =0;
                dy = -1;
                int t = Math.Max(_mapSize, _mapSize);
                int maxI = t*t;
                for(int i =0; i < maxI; i++){
                    if ((-_mapSize/2 <= x) && (x <= _mapSize/2) && (-_mapSize/2 <= y) && (y <= _mapSize/2))
                    {
                        var localPosition = (
                            x + (_mapSize % 2 == 0 ? _mapSize / 2 - 1 : _mapSize / 2),
                            y + (_mapSize % 2 == 0 ? _mapSize / 2 - 1 : _mapSize / 2));
                        
                        var position = (localPosition.Item1 + _chunkPosition.Item1 - (_mapSize/2), localPosition.Item2 + _chunkPosition.Item2 - (_mapSize/2));
                        
                        foreach (var chunk in oldChunks)
                        {
                            if (chunk != null)
                            {
                                if (chunk.GetPositionX() == position.Item1 && chunk.GetPositionY() == position.Item2)
                                {
                                    _mapChunkTexture2D[localPosition.Item1, localPosition.Item2] = chunk;
                                }
                            }
                        }

                        if (_mapChunkTexture2D[localPosition.Item1, localPosition.Item2] == null)
                        {
                            newChunkPositions.Add(localPosition);
                            newChunkLocations.Add(position);
                        }
                    }
                    if( (x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1-y))){
                        t = dx;
                        dx = -dy;
                        dy = t;
                    }
                    x += dx;
                    y += dy;
                }   
                
                for (var i = 0; i < newChunkPositions.Count; i++)
                    _mapChunkTexture2D[newChunkPositions[i].Item1, newChunkPositions[i].Item2] = new MapChunkTexture2D(_graphicsDevice, TerrainEntity.GenerateChunkData(newChunkLocations[i].Item1, newChunkLocations[i].Item2), 0, MapChunkTexture2D.Resolution.High);
                
            }

            _isLoading = false;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            LoadTextureChunks();
        }

        public void Update(GameTime gameTime)
        {
            var playerPosition = _gameEntityManager.GetPlayerEntity().GetPosition();

            _positionOffset = ((int, int)) (playerPosition.X%64, playerPosition.Z%64);
            
            var mapHasMoved = false;

            if (!_isLoading)
            {
                var newChunkPosition = ChunkLoaderEntity.CalculateChunkPosition(playerPosition);
                
                if (!_chunkPosition.Equals(newChunkPosition))
                {
                    _chunkPosition = newChunkPosition;
                    mapHasMoved = true;
                }

                if (mapHasMoved)
                {
#if EDITOR
                    Application.Invoke((sender, args) => LoadTextureChunks());           
#else
                    //Use this instead of Application.Invoke when not using the GTK editor
                    Thread thread = new Thread(LoadTextureChunks);
                    thread.Start();
#endif
                }
            }
        }
    }
}