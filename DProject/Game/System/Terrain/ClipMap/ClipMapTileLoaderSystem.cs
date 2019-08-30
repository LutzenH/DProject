using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DProject.Type.Rendering.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Terrain.ClipMap
{
    public class ClipMapTileLoaderSystem : UpdateSystem
    {
        private static ConcurrentDictionary<(int, int), ClipMapTerrainTile>[] _loadedTiles;
        
        private readonly Vector2[] _previousLensPosition;
        private readonly Vector2[] _lensPosition;

        private GraphicsDevice _graphicsDevice;

        public ClipMapTileLoaderSystem(GraphicsDevice graphicsDevice)
        {
            _loadedTiles = new ConcurrentDictionary<(int, int), ClipMapTerrainTile>[TerrainRenderSystem.ClipMapLevels];

            for (var i = 0; i < _loadedTiles.Length; i++)
                _loadedTiles[i] = new ConcurrentDictionary<(int, int), ClipMapTerrainTile>();
            
            _previousLensPosition = new Vector2[TerrainRenderSystem.ClipMapLevels];
            _lensPosition = new Vector2[TerrainRenderSystem.ClipMapLevels];

            _graphicsDevice = graphicsDevice;
        }

        public override void Update(GameTime gameTime)
        {
            for (var l = TerrainRenderSystem.ClipMapLevels - 1; l >= 0; l--)
            {
                float scale = 1 << l;
                
                _previousLensPosition[l] = _lensPosition[l];
                _lensPosition[l] = new Vector2(
                    (float) (Math.Floor(CameraSystem.ActiveLens.Position.X / scale) * scale),
                    (float) (Math.Floor(CameraSystem.ActiveLens.Position.Z / scale) * scale));

                if (!_previousLensPosition[l].Equals(_lensPosition[l]))
                    LoadTiles(_lensPosition[l], l);
            }
        }

        private void LoadTiles(Vector2 lensPosition, int clipMapLevel)
        {
            var posX = lensPosition.X / (256 * (clipMapLevel+1));
            var posY = lensPosition.Y / (256 * (clipMapLevel+1));
            
            var nearbyChunks = new (int, int)[4];
            
            nearbyChunks[0] = ((int)Math.Round(posX)-1, (int)Math.Round(posY)-1);
            nearbyChunks[1] = ((int)Math.Round(posX), (int)Math.Round(posY)-1);
            nearbyChunks[2] = ((int)Math.Round(posX)-1, (int)Math.Round(posY));
            nearbyChunks[3] = ((int)Math.Round(posX), (int)Math.Round(posY));

            for (var i = 0; i < nearbyChunks.Length; i++)
            {
                if (_loadedTiles[clipMapLevel].ContainsKey((nearbyChunks[i].Item1, nearbyChunks[i].Item2)))
                    continue;
                
                var filePathDiffuse = $"{Game1.RootDirectory}chunks/diffuse/{clipMapLevel}/diffuse_{nearbyChunks[i].Item1}_{nearbyChunks[i].Item2}.png";
                var filePathHeight = $"{Game1.RootDirectory}chunks/height/{clipMapLevel}/height_{nearbyChunks[i].Item1}_{nearbyChunks[i].Item2}.png";
                var filePathNormal = $"{Game1.RootDirectory}chunks/normal/{clipMapLevel}/normal_{nearbyChunks[i].Item1}_{nearbyChunks[i].Item2}.png";

                var diffuseExists = File.Exists(filePathDiffuse);
                var heightExists = File.Exists(filePathHeight);
                var normalExists = File.Exists(filePathNormal);

                try
                {
                    var diffuse = diffuseExists ? new Bitmap(filePathDiffuse) : null;
                    var height = heightExists ? new Bitmap(filePathHeight) : null;
                    var normal = normalExists ? new Bitmap(filePathNormal) : null;

                    _loadedTiles[clipMapLevel][(nearbyChunks[i].Item1, nearbyChunks[i].Item2)] = new ClipMapTerrainTile()
                    {
                        Origin = new Vector2(nearbyChunks[i].Item1 * 256, nearbyChunks[i].Item2 * 256),
                        Size = new Vector2(256),
                        
                        Diffuse = ConvertToTexture(diffuse, _graphicsDevice),
                        Height = ConvertToTexture(height, _graphicsDevice),
                        Normal = ConvertToTexture(normal, _graphicsDevice)
                    };
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    throw;
                }
            }

            var unloadedChunks = _loadedTiles[clipMapLevel].Keys.Except(nearbyChunks);
            
            foreach (var chunk in unloadedChunks)
            {
                _loadedTiles[clipMapLevel].TryRemove(chunk, out var unloadedChunk);
                unloadedChunk.Dispose();
            }
        }

        public static IEnumerable<ClipMapTerrainTile> GetLoadedTiles(int clipMapLevel)
        {
            if(clipMapLevel > TerrainRenderSystem.ClipMapLevels)
                throw new ArgumentOutOfRangeException($"The given value {clipMapLevel} is higher than the amount of clipMapLevels: {TerrainRenderSystem.ClipMapLevels}");
            
            return _loadedTiles[clipMapLevel].Values;
        }

        private static Texture2D ConvertToTexture(Bitmap bitmap, GraphicsDevice graphicsDevice)
        {
            if (bitmap == null)
                return null;
            
            Texture2D texture2D;
            
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                texture2D = Texture2D.FromStream(graphicsDevice, memoryStream);
            }
            
            return texture2D;
        }
    }
}
