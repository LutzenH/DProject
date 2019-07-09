using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Manager
{
    public class ShaderManager
    {
        private static ContentManager _contentManager;
        private static GraphicsDevice _graphicsDevice;

        private static TerrainEffect _terrainEffect;
        
        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
        }

        public static TerrainEffect TerrainEffect => _terrainEffect ?? (_terrainEffect = new TerrainEffect(_graphicsDevice));
    }
} 
