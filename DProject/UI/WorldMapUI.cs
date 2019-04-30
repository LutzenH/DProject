using System;
using System.Collections.Generic;
#if EDITOR
using Gtk;
#else
using System.Threading;
#endif
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
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
        private readonly ChunkLoaderEntity _chunkLoaderEntity;

        private GraphicsDevice _graphicsDevice;

        private MapChunkTexture2D[,] _mapChunkTexture2D;
        
        private bool _previouslyLoadingChunks;

        private int _chunkPositionX;
        private int _chunkPositionY;

        private bool _isLoading;

        private int _mapSize;

        public WorldMapUI(ChunkLoaderEntity chunkLoaderEntity, int mapSize)
        {
            _chunkLoaderEntity = chunkLoaderEntity;
            _mapSize = mapSize;
            
            _mapChunkTexture2D = new MapChunkTexture2D[_mapSize, _mapSize];
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
            
            if (!_chunkLoaderEntity.IsLoadingChunks())
            {
                int oldChunksCount = 0;
                int newChunksCount = 0;

                MapChunkTexture2D[,] oldChunks = _mapChunkTexture2D;

                _mapChunkTexture2D = new MapChunkTexture2D[_mapSize, _mapSize];
                
                List<Vector2> newChunkPositions = new List<Vector2>();
                List<Vector2> newChunkLocations = new List<Vector2>();
                
                int x,y,dx,dy;
                x = y = dx =0;
                dy = -1;
                int t = Math.Max(_mapSize, _mapSize);
                int maxI = t*t;
                for(int i =0; i < maxI; i++){
                    if ((-_mapSize/2 <= x) && (x <= _mapSize/2) && (-_mapSize/2 <= y) && (y <= _mapSize/2))
                    {
                        var localx = x + (_mapSize % 2 == 0 ? _mapSize/2-1 : _mapSize/2);
                        var localy = y + (_mapSize % 2 == 0 ? _mapSize/2-1 : _mapSize/2);

                        Vector2 position = new Vector2(localx + _chunkPositionX, localy + _chunkPositionY);
                        
                        foreach (var chunk in oldChunks)
                        {
                            if (chunk != null)
                            {
                                if (chunk.GetPositionX() == (int) position.X && chunk.GetPositionY() == (int) position.Y)
                                {
                                    _mapChunkTexture2D[localx, localy] = chunk;
                                    oldChunksCount++;
                                }
                            }
                        }

                        if (_mapChunkTexture2D[localx, localy] == null)
                        {
                            newChunksCount++;
                            newChunkPositions.Add(new Vector2(localx, localy));
                            newChunkLocations.Add(new Vector2(position.X, position.Y));
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
                
                for (int i = 0; i < newChunkPositions.Count; i++)
                {
                    _mapChunkTexture2D[(int) newChunkPositions[i].X, (int) newChunkPositions[i].Y] = new MapChunkTexture2D(_graphicsDevice, TerrainEntity.GenerateChunkData((int) newChunkLocations[i].X,(int) newChunkLocations[i].Y), 0, MapChunkTexture2D.Resolution.High);
                }
                
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
            var mapHasMoved = false;

            if (!_isLoading)
            {
                
                if (Keyboard.GetState().IsKeyUp(Keys.NumPad8) && Game1.PreviousKeyboardState.IsKeyDown(Keys.NumPad8))
                {
                    _chunkPositionY--;
                    mapHasMoved = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.NumPad2) && Game1.PreviousKeyboardState.IsKeyDown(Keys.NumPad2))
                {
                    _chunkPositionY++;
                    mapHasMoved = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.NumPad4) && Game1.PreviousKeyboardState.IsKeyDown(Keys.NumPad4))
                {
                    _chunkPositionX--;
                    mapHasMoved = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.NumPad6) && Game1.PreviousKeyboardState.IsKeyDown(Keys.NumPad6))
                {
                    _chunkPositionX++;
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