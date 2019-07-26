using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type;
using MessagePack;
using Microsoft.Xna.Framework.Graphics;
using Texture = DProject.Type.Serializable.Texture;

namespace DProject.List
{
    public static class Textures
    {
        private const string TextureListPath = "collections/textures.json";
        
        public static readonly Dictionary<string, TextureAtlas> AtlasList = new Dictionary<string, TextureAtlas>();
        
        public static readonly Dictionary<ushort, string> FloorTextureIdentifiers = new Dictionary<ushort, string>();

        static Textures()
        {            
            using (TextReader reader = new StreamReader(Game1.RootDirectory + TextureListPath))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var textureAtlasList = MessagePackSerializer.Deserialize<Dictionary<string, Dictionary<string, Texture>>>(bytes);

                foreach (var list in textureAtlasList)
                {
                    var textureAtlas = new TextureAtlas(list.Key);

                    if (list.Key == "floor_textures")
                    {
                        ushort identifier = 0;
                        foreach (var texture in list.Value)
                        {
                            FloorTextureIdentifiers.Add(identifier++, texture.Key);
                            textureAtlas.TextureList.Add(texture.Key, texture.Value);
                        }
                    }
                    else
                    {
                        foreach (var texture in list.Value)
                            textureAtlas.TextureList.Add(texture.Key, texture.Value);
                    }

                    GenerateTextureAtlas(textureAtlas);

                    AtlasList.Add(list.Key, textureAtlas);
                }
            }
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            foreach (var atlas in AtlasList)
                atlas.Value.Initialize(graphicsDevice);
        }

        private static void GenerateTextureAtlas(TextureAtlas textureAtlas)
        {
            if (textureAtlas.TextureList.Count == 0)
                throw new ArgumentNullException(nameof(textureAtlas.TextureList), "Not enough textures in the TextureList");
            
            var imagePacker = new ImagePacker.ImagePacker();
            var textureLocationList = new List<string>();

            foreach (var texture in textureAtlas.TextureList)
            {
                textureLocationList.Add(Game1.RootDirectory + texture.Value.TexturePath);
            }
            
            imagePacker.PackImage(textureLocationList, false, true, textureAtlas.AtlasSize, textureAtlas.AtlasSize, 1, true, 
                out var outputImage,
                out var outputMap);

            foreach (var texture in textureAtlas.TextureList)
            {
                texture.Value.XSize = outputMap[Game1.RootDirectory + texture.Value.TexturePath].Width;
                texture.Value.YSize = outputMap[Game1.RootDirectory + texture.Value.TexturePath].Height;
                texture.Value.XOffset = outputMap[Game1.RootDirectory + texture.Value.TexturePath].X;
                texture.Value.YOffset = outputMap[Game1.RootDirectory + texture.Value.TexturePath].Y;
            }

            textureAtlas.AtlasSize = outputImage.Width;
            textureAtlas.AtlasBitmap = outputImage;
            
            Console.WriteLine("Generated a Texture Atlas from " + textureAtlas.TextureList.Count + " Textures in " + TextureListPath + ":" + textureAtlas.Name);
        }

        public static ushort GetTextureIdFromName(string name)
        {
            foreach (var identifier in FloorTextureIdentifiers)
            {
                if (identifier.Value.Equals(name))
                    return identifier.Key;
            }
            
            throw new ArgumentException("Could not find a texture identifier using the given string.");
        }
    }
}