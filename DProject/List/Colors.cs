using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using Gdk;
using MessagePack;

namespace DProject.List
{
    public static class Colors
    {
        private const string ColorListPath = "collections/colors.json";
        
        public static readonly Dictionary<ushort, SerializableColor> ColorList = new Dictionary<ushort, SerializableColor>();

        static Colors()
        {
            using (TextReader reader = new StreamReader(Game1.RootDirectory + ColorListPath))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var colorList = MessagePackSerializer.Deserialize<Dictionary<string,List<SerializableColor>>>(bytes);

                for (ushort i = 0; i < colorList["colors"].Count; i++)
                {
                    ColorList[i] = colorList["colors"][i];
                }
                
                Console.WriteLine("Retrieved " + colorList["colors"].Count + " Colors from " + ColorListPath);
            }
        }

        public static ushort GetDefaultColorId()
        {
            return 0;
        }
        
        public static ushort GetColorIdFromName(string name)
        {
            foreach (var color in ColorList)
            {
                if (color.Value.GetColorName() == name)
                    return color.Key;
            }

            throw new ArgumentException();
        }

        public static RGBA GetRgbaFromName(string name)
        {
            foreach (var color in ColorList)
            {
                if (color.Value.GetColorName() == name)
                    return new RGBA()
                    {
                        Red = color.Value.Red/256d,
                        Green = color.Value.Green/256d,
                        Blue = color.Value.Blue/256d,
                        Alpha = 1d
                    };
            }
            
            throw new ArgumentException();
        }

        public static void SetColorFromRgba(RGBA rgba, ushort colorId)
        {
            ColorList[colorId].SetColor(
                (byte) (rgba.Red * 255),
                (byte) (rgba.Green * 255),
                (byte) (rgba.Blue * 255)
                );
        }
        
        public static void SetColorFromRgba(RGBA rgba, string name)
        {
            SetColorFromRgba(rgba, GetColorIdFromName(name));
        }
    }

}