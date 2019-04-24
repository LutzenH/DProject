using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Texture = DProject.Type.Serializable.Texture;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<ushort, Texture> TextureList = new Dictionary<ushort, Texture>();

        public const string TextureAtlasLocation = "textures/textureatlas";
        public const int TextureAtlasSize = 128;

        public static Texture2D _terrainTexture;
        
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
            }
        }

        public static ushort GetDefaultTextureId()
        {
            return 0;
        }

        public static void LoadContent(ContentManager content)
        {
            _terrainTexture = content.Load<Texture2D>(TextureAtlasLocation);
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