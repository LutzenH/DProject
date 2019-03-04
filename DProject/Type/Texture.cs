using DProject.List;
using Microsoft.Xna.Framework;

namespace DProject.Type
{
    public class Texture
    {
        private readonly Vector4 _texturePosition;
        private readonly Color _defaultColor;

        public Texture(int xOffset, int yOffset, int xSize, int ySize, Color defaultColor)
        {
            _texturePosition = new Vector4(xOffset, yOffset, xOffset+xSize, yOffset+ySize);
            _defaultColor = defaultColor;
        }

        public Texture(int xOffset, int yOffset, int xSize, int ySize) : this(xOffset, yOffset, xSize, ySize, new Color(1f, 1f, 1f)) { }

        public Vector4 GetTexturePosition()
        {
            return _texturePosition;
        }

        public Color GetDefaultColor()
        {
            return _defaultColor;
        }

        public Vector4 GetAdjustedTexturePosition()
        {
            return _texturePosition / Textures.TextureAtlasSize;
        }
    }
}