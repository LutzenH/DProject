using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DProject.Type.Serializable;
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

        public static void ExportColorListToJson()
        {
            var dictionary = new Dictionary<string, List<SerializableColor>>();
            dictionary.Add("colors", ColorList.Values.ToList());
            var json = MessagePackSerializer.ToJson(dictionary);
            
            using (var newTask = new StreamWriter(Game1.RootDirectory + ColorListPath, false)) 
                newTask.WriteLine(json);    
        }

        public static ushort GetColorIdFromName(string name)
        {
            foreach (var color in ColorList)
            {
                if (color.Value.Name == name)
                    return color.Key;
            }

            throw new ArgumentException();
        }
    }

}