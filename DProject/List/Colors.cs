using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using MessagePack;

namespace DProject.List
{
    public static class Colors
    {
        public static readonly Dictionary<ushort, SerializableColor> ColorList = new Dictionary<ushort, SerializableColor>();

        static Colors()
        {
            using (TextReader reader = new StreamReader("Content/collections/colors.json"))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var colorList = MessagePackSerializer.Deserialize<Dictionary<string,List<SerializableColor>>>(bytes);

                for (ushort i = 0; i < colorList["colors"].Count; i++)
                {
                    ColorList[i] = colorList["colors"][i];
                }
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
    }

}