using System;
using System.Drawing;
using System.Drawing.Imaging;
using DProject.Game.Component.Terrain.ClipMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Terrain.ClipMap
{
    public class ClipMapLoaderSystem : EntityUpdateSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<ClipMapComponent> _clipMapMapper;
        private ComponentMapper<LoadedClipMapComponent> _loadedClipMapMapper;

        public const int TextureSize = 256;

        public ClipMapLoaderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(ClipMapComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _loadedClipMapMapper = mapperService.GetMapper<LoadedClipMapComponent>();
            _clipMapMapper = mapperService.GetMapper<ClipMapComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var clipMap = _clipMapMapper.Get(entity);

                var loadedClipMapComponent = new LoadedClipMapComponent
                {
                    Type = clipMap.Type,
                    ClipMap = new RenderTarget2D[TerrainRenderSystem.ClipMapLevels]
                };

                for (var i = 0; i < TerrainRenderSystem.ClipMapLevels; i++)
                    loadedClipMapComponent.ClipMap[i] = new RenderTarget2D(_graphicsDevice, TextureSize, TextureSize);
                
                loadedClipMapComponent.ViewportSize = new Vector2(TextureSize, TextureSize);
                
                _loadedClipMapMapper.Put(entity, loadedClipMapComponent);
                _clipMapMapper.Delete(entity);
            }
        }
        
        public static void ConvertImageToTiles(string filePath, string outputPath, string outputName, int depth)
        {
            Console.WriteLine("Loading full image...");

            var bitmap = new Bitmap(filePath);

            var bitmapWidth = bitmap.Width;
            var bitmapHeight = bitmap.Height;
            
            Console.WriteLine("Loaded full image: " + bitmapWidth + "x" + bitmapHeight);

            for (var tileXPos = 0; tileXPos < bitmapWidth; tileXPos+=TextureSize)
            {
                for (var tileYPos = 0; tileYPos < bitmapHeight; tileYPos+=TextureSize)
                {
                    var tile = new Bitmap(TextureSize, TextureSize, PixelFormat.Format24bppRgb);

                    for (var x = 0; x < TextureSize; x++)
                    {
                        for (var y = 0; y < TextureSize; y++)
                        {
                            tile.SetPixel(x, y, bitmap.GetPixel(tileXPos + x,tileYPos + y));
                        }
                    }
                    
                    tile.Save($"{outputPath}{outputName}_{tileXPos/TextureSize}_{tileYPos/TextureSize}.png", ImageFormat.Png);
                    Console.WriteLine($"Saving tile {depth}: {outputName}_{tileXPos/TextureSize}_{tileYPos/TextureSize}.png");
                }
            }
            
            Console.WriteLine("Done.");
        }
    }
}
