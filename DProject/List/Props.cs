using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using MessagePack;

namespace DProject.List
{
    public static class Props
    {
        private const string PropListPath = "collections/props.json";
        
        public static readonly Dictionary<ushort, Prop> PropList = new Dictionary<ushort, Prop>();

        static Props()
        {
            using (TextReader reader = new StreamReader(Game1.RootDirectory + PropListPath))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var propList = MessagePackSerializer.Deserialize<Dictionary<string,List<Prop>>>(bytes);

                for (ushort i = 0; i < propList["props"].Count; i++)
                {
                    PropList[i] = propList["props"][i];
                }
                
                Console.WriteLine("Retrieved " + propList["props"].Count + " Props from " + PropListPath);
            }
        }
        
        public static ushort GetDefaultPropId()
        {
            return 0;
        }
        
        public static ushort GetPropIdFromName(string name)
        {
            foreach (var prop in PropList)
            {
                if (prop.Value.Name == name)
                    return prop.Key;
            }

            throw new ArgumentException();
        }
    }

}