using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using MessagePack;

namespace DProject.List
{
    public static class Props
    {
        public static readonly Dictionary<ushort, Prop> PropList = new Dictionary<ushort, Prop>();

        static Props()
        {
            using (TextReader reader = new StreamReader("Content/collections/props.json"))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var propList = MessagePackSerializer.Deserialize<Dictionary<string,List<Prop>>>(bytes);

                for (ushort i = 0; i < propList["props"].Count; i++)
                {
                    PropList[i] = propList["props"][i];
                }
            }
        }
        
        public static ushort GetDefaultPropId()
        {
            return 0;
        }
        
        public static ushort GetPropIdFromName(string name)
        {
            foreach (var texture in PropList)
            {
                if (texture.Value.Name == name)
                    return texture.Key;
            }

            throw new ArgumentException();
        }
    }

}