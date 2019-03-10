using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<string, Texture> TextureList = new Dictionary<string, Texture>();

        public const string TextureAtlasLocation = "textures/textureatlas";
        public const int TextureAtlasSize = 128;

        static Textures()
        {
            //Random
            TextureList["tile_white"] = new Texture(0,0,16,16);
            TextureList["planks"] = new Texture(16,0,16,16);
            TextureList["metal_floor"] = new Texture(32,0,16,16);
            TextureList["metal"] = new Texture(48,0,16,16);
            TextureList["grass"] = new Texture(64,0,16,16);
            
            //Savanna
            TextureList["savanna_grass"] = new Texture(0, 16, 16, 16);
            TextureList["savanna_grass_dry"] = new Texture(16, 16, 16, 16);
            TextureList["savanna_earth"] = new Texture(32, 16, 16, 16);
            
            //Road
            TextureList["road_longtile_front"] = new Texture(0, 32, 16, 16);
            TextureList["road_longtile_mid"] = new Texture(16, 32, 16, 16);
            TextureList["road_longtile_end"] = new Texture(32, 32, 16, 16);
            TextureList["road_0"] = new Texture(0, 48, 16, 16);
            TextureList["road_1"] = new Texture(16, 48, 16, 16);
            TextureList["road_2"] = new Texture(32, 48, 16, 16);
            TextureList["road_sidewalk"] = new Texture(0, 64, 16, 16);
            TextureList["sidewalk_tile_0"] = new Texture(16, 64, 16, 16);
            TextureList["sidewalk_tile_1"] = new Texture(32, 64, 16, 16);
            TextureList["sidewalk_tile_2"] = new Texture(48, 32, 16, 16);
        }
    }
}