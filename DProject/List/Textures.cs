using System;
using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<ushort, Texture> TextureList = new Dictionary<ushort, Texture>();

        public const string TextureAtlasLocation = "textures/textureatlas";
        public const int TextureAtlasSize = 128;

        static Textures()
        {
            //Random
            TextureList[0] = new Texture(0, 0, 16, 16, "tile_white");
            TextureList[1] = new Texture(16,0,16,16,"planks");
            TextureList[2] = new Texture(32,0,16,16,"metal_floor");
            TextureList[3] = new Texture(48,0,16,16,"metal");
            TextureList[4] = new Texture(64,0,16,16,"grass");
            
            //Savanna
            TextureList[5] = new Texture(0, 16, 16, 16,"savanna_grass");
            TextureList[6] = new Texture(16, 16, 16, 16,"savanna_grass_dry");
            TextureList[7] = new Texture(32, 16, 16, 16,"savanna_earth");
            
            //Road
            TextureList[8] = new Texture(0, 32, 16, 16,"road_longtile_front");
            TextureList[9] = new Texture(16, 32, 16, 16,"road_longtile_mid");
            TextureList[10] = new Texture(32, 32, 16, 16,"road_longtile_end");
            TextureList[11] = new Texture(0, 48, 16, 16,"road_0");
            TextureList[12] = new Texture(16, 48, 16, 16,"road_1");
            TextureList[13] = new Texture(32, 48, 16, 16,"road_2");
            TextureList[14] = new Texture(0, 64, 16, 16,"road_sidewalk");
            TextureList[15] = new Texture(16, 64, 16, 16,"sidewalk_tile_0");
            TextureList[16] = new Texture(32, 64, 16, 16,"sidewalk_tile_1");
            TextureList[17] = new Texture(48, 32, 16, 16,"sidewalk_tile_2");
        }

        public static ushort GetDefaultTextureId()
        {
            return 0;
        }

        public static ushort GetTextureIdFromName(string name)
        {
            foreach (var (key, value) in TextureList)
            {
                if (value.GetTextureName() == name)
                    return key;
            }

            throw new ArgumentException();
        }
    }
}