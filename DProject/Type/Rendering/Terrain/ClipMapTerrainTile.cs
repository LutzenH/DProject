using DProject.Manager.System.Terrain.ClipMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Terrain
{
    public class ClipMapTerrainTile
    {
        public Vector2 Origin { get; set; }
        public Vector2 Size { get; set; }

        public Texture2D Diffuse { get; set; }
        public Texture2D Height { get; set; }
        public Texture2D Normal { get; set; }

        public void Dispose()
        {
            if(Diffuse != ClipMapTileLoaderSystem.DiffuseTilePlaceHolder)
                Diffuse?.Dispose();
            
            if(Height != ClipMapTileLoaderSystem.HeightTilePlaceHolder)
                Height?.Dispose();
            
            if(Normal != ClipMapTileLoaderSystem.NormalTilePlaceHolder)
                Normal?.Dispose();
        }
    }
}
