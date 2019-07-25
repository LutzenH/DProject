using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Texture = DProject.Type.Serializable.Texture;

namespace DProject.Type
{
    public class TextureAtlas
    {
        public string Name { get; set; }

        public int AtlasSize { get; set; }

        public Bitmap AtlasBitmap { get; set; }
        public Texture2D AtlasTexture2D { get; set; }

        public readonly Dictionary<string, Texture> TextureList;

        public TextureAtlas(string name)
        {
            Name = name;
            AtlasSize = 2048;
            TextureList = new Dictionary<string, Texture>();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            AtlasTexture2D = ConvertToTexture(AtlasBitmap, graphicsDevice);
        }
        
        private static Texture2D ConvertToTexture(Bitmap bitmap, GraphicsDevice graphicsDevice)
        {
            Texture2D texture2D;
            
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                texture2D = Texture2D.FromStream(graphicsDevice, memoryStream);
            }
            
            return texture2D;
        }
    }
}
