using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;
using DProject.Type.Serializable.Chunk;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.List
{
    public static class Skies
    {
        private const string SkyListPath = "collections/skies.json";
        
        public static readonly Dictionary<ushort, Sky> SkyList = new Dictionary<ushort, Sky>();

        static Skies()
        {
            using (TextReader reader = new StreamReader(Game1.RootDirectory + SkyListPath))
            {
                var text = reader.ReadToEnd();
                var bytes = MessagePackSerializer.FromJson(text);
                var skyList = MessagePackSerializer.Deserialize<Dictionary<string,List<Sky>>>(bytes);

                for (ushort i = 0; i < skyList["skies"].Count; i++)
                {
                    SkyList[i] = skyList["skies"][i];
                }
                
                Console.WriteLine("Retrieved " + skyList["skies"].Count + " Skies from " + SkyListPath);
            }
        }

        public static ushort GetDefaultSkyId()
        {
            return 0;
        }
        
        public static ushort GetSkyIdFromName(string name)
        {
            foreach (var sky in SkyList)
            {
                if (sky.Value.Name == name)
                    return sky.Key;
            }

            throw new ArgumentException();
        }
        
        public static readonly Sky DefaultSky = new Sky()
        {
            Name = "Default",
            
            Fog = new Fog()
            {
                FogStart = 120f,
                FogEnd = 160f,
                
                Color = new SerializableColor()
                {
                    Color = Color.DarkGray
                }
            },

            BackgroundColor = new SerializableColor()
            {
                Color = Color.DarkGray
            },
            
            AmbientLightColor = new SerializableColor()
            {
                Color = Color.Black
            },

            DirectionalLight0 = new DirectionalLight()
            {
                DiffuseColor = new SerializableColor()
                {
                    Color = Color.White
                },
                SpecularColor = new SerializableColor()
                {
                    Color = Color.Black
                },
                Direction = Vector3.Normalize(Vector3.Down)
            }
        };
        
        public static void ExportDefaultSkyToJson()
        {
            var json = MessagePackSerializer.ToJson(DefaultSky);
            
            using (var newTask = new StreamWriter(Game1.RootDirectory + "collections/sky.json", false)) 
                newTask.WriteLine(json);    
        }
    }
} 
