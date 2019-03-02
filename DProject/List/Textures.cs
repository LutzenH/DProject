using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Textures
    {
        public static readonly Dictionary<string, Texture> TextureList = new Dictionary<string, Texture>();

        static Textures()
        {
            TextureList["savanna_grass"] = new Texture("textures/savanna-grass");
            TextureList["savanna_grass_dry"] = new Texture("textures/savanna-dry_grass");
            TextureList["savanna_earth"] = new Texture("textures/savanna-dry_earth");
        }
    }
}