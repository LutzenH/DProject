using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using MessagePack;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.List
{
    public static class Fonts
    {
        private const string FontListPath = "collections/fonts.json";

        public static readonly Dictionary<string, Dictionary<string, Font>> FontList;

        static Fonts()
        {
            using (TextReader reader = new StreamReader(Game1.RootDirectory + FontListPath))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                
                FontList = MessagePackSerializer.Deserialize<Dictionary<string,Dictionary<string, Font>>>(bytes);

                foreach (var fonts in FontList)
                    Console.WriteLine("Retrieved " + fonts.Value.Count + " Fonts from " + FontListPath + ":" + fonts.Key);
            }
        }

        public static void LoadFonts(ContentManager content)
        {
            foreach (var fonts in FontList)
            {
                foreach (var font in fonts.Value)
                {
                    font.Value.SpriteFont = content.Load<SpriteFont>(font.Value.Location);
                }
            }
        }
    }
} 
