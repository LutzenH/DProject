using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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
            AtlasTexture2D = new Texture2D(graphicsDevice, AtlasBitmap.Width, AtlasBitmap.Height, false, SurfaceFormat.Color);
            
            var data = AtlasBitmap.LockBits(new Rectangle(0, 0, AtlasBitmap.Width, AtlasBitmap.Height), ImageLockMode.ReadOnly, AtlasBitmap.PixelFormat);
            var bytes = new byte[data.Height * data.Stride];    

            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            
            AtlasTexture2D.SetData(bytes);
        }
    }
}
