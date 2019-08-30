using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class ClipMapEffect : Effect
    {
        public ClipMapEffect(Effect cloneSource) : base(cloneSource) { }

        /// <summary>
        /// The viewport size in pixels.
        /// </summary>
        public Vector2 ViewportSize {             
            get => Parameters["ViewportSize"].GetValueVector2();
            set => Parameters["ViewportSize"].SetValue(value);
        }
        
        /// <summary>
        /// World space origin of this tile.
        /// </summary>
        public Vector2 TerrainTileOrigin {             
            get => Parameters["TerrainTileOrigin"].GetValueVector2();
            set => Parameters["TerrainTileOrigin"].SetValue(value);
        }
        
        /// <summary>
        /// World space size of this tile.
        /// </summary>
        public Vector2 TerrainTileSize {             
            get => Parameters["TerrainTileSize"].GetValueVector2();
            set => Parameters["TerrainTileSize"].SetValue(value);
        }
        
        /// <summary>
        /// The clipmap level which is currently being rendered.
        /// </summary>
        public float TerrainClipMapLevel {             
            get => Parameters["TerrainClipMapLevel"].GetValueSingle();
            set => Parameters["TerrainClipMapLevel"].SetValue(value);
        }

        /// <summary>
        /// The size in pixels of the Tile Texture that has to be rendered.
        /// </summary>
        public Vector2 TerrainTileTextureSize {             
            get => Parameters["TerrainTileTextureSize"].GetValueVector2();
            set => Parameters["TerrainTileTextureSize"].SetValue(value);
        }
        
        /// <summary>
        /// The Tile Texture that has to be rendered.
        /// </summary>
        public Texture2D TerrainTileTexture
        {
            get => Parameters["TerrainTileTexture"].GetValueTexture2D();
            set => Parameters["TerrainTileTexture"].SetValue(value);
        }
    }
}
