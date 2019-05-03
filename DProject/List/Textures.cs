using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack;
using Microsoft.Xna.Framework.Graphics;
using Texture = DProject.Type.Serializable.Texture;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<ushort, Texture> TextureList = new Dictionary<ushort, Texture>();

        //public const string TextureAtlasLocation = "textures/textureatlas";
        public static int TextureAtlasSize = 2048;

        public static Bitmap TextureAtlas;
        public static Texture2D TerrainTexture;
        
        static Textures()
        {            
            using (TextReader reader = new StreamReader("Content/collections/textures.json"))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var textureList = MessagePackSerializer.Deserialize<Dictionary<string,List<Texture>>>(bytes);

                for (ushort i = 0; i < textureList["textures"].Count; i++)
                {
                    TextureList[i] = textureList["textures"][i];
                }

                GenerateTextureAtlas();
            }
        }
        	
        private static void GenerateTextureAtlas()
        {
            string localpath = "Content/";
            
            if (TextureList.Count == 0)
                throw new ArgumentNullException(nameof(TextureList), "Not enough textures in the TextureList");
            
            ImagePacker.ImagePacker imagePacker = new ImagePacker.ImagePacker();

            var textureLocationList = new List<string>();

            foreach (var texture in TextureList)
            {
                textureLocationList.Add(localpath + texture.Value.TexturePath);
            }
            
            imagePacker.PackImage(textureLocationList, false, true, TextureAtlasSize, TextureAtlasSize, 0, true, 
                out var outputImage,
                out var outputMap);

            foreach (var texture in TextureList)
            {
                texture.Value.XSize = outputMap[localpath + texture.Value.TexturePath].Width;
                texture.Value.YSize = outputMap[localpath + texture.Value.TexturePath].Height;
                texture.Value.XOffset = outputMap[localpath + texture.Value.TexturePath].X;
                texture.Value.YOffset = outputMap[localpath + texture.Value.TexturePath].Y;
            }

            TextureAtlasSize = outputImage.Width;
            TextureAtlas = outputImage;
            
            Console.WriteLine("Done generating a Texture Atlas for collections/textures.json");
        }

        public static ushort GetDefaultTextureId()
        {
            return 0;
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            TerrainTexture = new Texture2D(graphicsDevice, TextureAtlas.Width, TextureAtlas.Height, false, SurfaceFormat.Color);
            
            BitmapData data = TextureAtlas.LockBits(new Rectangle(0, 0, TextureAtlas.Width, TextureAtlas.Height), ImageLockMode.ReadOnly, TextureAtlas.PixelFormat);
            
            byte[] bytes = new byte[data.Height * data.Stride];    

            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            
            TerrainTexture.SetData(bytes);
        }

        public static ushort GetTextureIdFromName(string name)
        {
            foreach (var texture in TextureList)
            {
                if (texture.Value.GetTextureName() == name)
                    return texture.Key;
            }

            throw new ArgumentException();
        }
    }
}