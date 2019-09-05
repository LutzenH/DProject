using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DProject.Game.Component.Terrain.ClipMap;
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

        public static Texture2D DiffuseTilePlaceHolder;
        public static Texture2D HeightTilePlaceHolder;
        public static Texture2D NormalTilePlaceHolder;

        public ClipMapTileLoaderSystem(GraphicsDevice graphicsDevice)
        {
            _loadedTiles = new ConcurrentDictionary<(int, int), ClipMapTerrainTile>[TerrainRenderSystem.ClipMapLevels];

            for (var i = 0; i < _loadedTiles.Length; i++)
                _loadedTiles[i] = new ConcurrentDictionary<(int, int), ClipMapTerrainTile>();
            
            _previousLensPosition = new Vector2[TerrainRenderSystem.ClipMapLevels];
            _lensPosition = new Vector2[TerrainRenderSystem.ClipMapLevels];

            _graphicsDevice = graphicsDevice;

            DiffuseTilePlaceHolder = ConvertToTexture(new Bitmap(Game1.RootDirectory + "chunks/diffuse/unloaded.png"), graphicsDevice);
            HeightTilePlaceHolder = ConvertToTexture(new Bitmap(Game1.RootDirectory + "chunks/height/unloaded.png"), graphicsDevice);
            NormalTilePlaceHolder = ConvertToTexture(new Bitmap(Game1.RootDirectory + "chunks/normal/unloaded.png"), graphicsDevice);
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
            var scale = 256 << clipMapLevel;
            
            var posX = lensPosition.X;
            var posY = lensPosition.Y;
            
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
                    var diffuse = diffuseExists ? ConvertToTexture(new Bitmap(filePathDiffuse), _graphicsDevice) : DiffuseTilePlaceHolder;
                    var height = heightExists ? ConvertToTexture(new Bitmap(filePathHeight), _graphicsDevice) : HeightTilePlaceHolder;
                    var normal = normalExists ? ConvertToTexture(new Bitmap(filePathNormal), _graphicsDevice) : NormalTilePlaceHolder;

                    _loadedTiles[clipMapLevel][(nearbyChunks[i].Item1, nearbyChunks[i].Item2)] = new ClipMapTerrainTile
                    {
                        Origin = new Vector2(nearbyChunks[i].Item1 * scale, nearbyChunks[i].Item2 * scale),
                        Size = new Vector2(scale),
                        
                        Diffuse = diffuse,
                        Height = height,
                        Normal = normal
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

        public static Texture2D GetFirstTextureInClipMapLevel(int clipMapLevel, ClipMapType type)
        {
            if(clipMapLevel > TerrainRenderSystem.ClipMapLevels)
                throw new ArgumentOutOfRangeException($"The given value {clipMapLevel} is higher than the amount of clipMapLevels: {TerrainRenderSystem.ClipMapLevels}");

            if (_loadedTiles[clipMapLevel].Values.Count > 0)
            {
                switch (type)
                {
                    case ClipMapType.Height:
                        return _loadedTiles[clipMapLevel].Values.Last().Height;
                    case ClipMapType.Normal:
                        return _loadedTiles[clipMapLevel].Values.Last().Normal;
                    case ClipMapType.Diffuse:
                        return _loadedTiles[clipMapLevel].Values.Last().Diffuse;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, "The type given does not exist.");
                }
            }
            
            return null;
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
