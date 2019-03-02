using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<string, Texture> TextureList = new Dictionary<string, Texture>();

        public static readonly string texture_atlas_location = "textures/textureatlas";

        public static readonly int texture_atlas_size = 128;
        
        static Textures()
        {
            TextureList["tile_white"] = new Texture(0,0,16,16);
            TextureList["planks"] = new Texture(16,0,16,16);
            TextureList["metal_floor"] = new Texture(32,0,16,16);
            TextureList["metal"] = new Texture(48,0,16,16);
            TextureList["grass"] = new Texture(64,0,16,16);
            
            TextureList["savanna_grass"] = new Texture(0, 16, 16, 16);
            TextureList["savanna_grass_dry"] = new Texture(16, 16, 16, 16);
            TextureList["savanna_earth"] = new Texture(32, 16, 16, 16);
        }
    }
}