using DProject.List;
using Microsoft.Xna.Framework;

namespace DProject.Type
{
    public class Texture
    {
        private readonly Vector4 _texturePosition;
        private readonly Color _defaultColor;
        private readonly string _textureName;

        public Texture(int xOffset, int yOffset, int xSize, int ySize, string textureName, Color defaultColor)
        {
            _texturePosition = new Vector4(xOffset, yOffset, xOffset+xSize, yOffset+ySize);
            _defaultColor = defaultColor;
            _textureName = textureName;
        }

        public Texture(int xOffset, int yOffset, int xSize, int ySize, string textureName) : this(xOffset, yOffset, xSize, ySize, textureName, new Color(1f, 1f, 1f)) { }

        public Vector4 GetTexturePosition()
        {
            return _texturePosition;
        }

        public Color GetDefaultColor()
        {
            return _defaultColor;
        }

        public string GetTextureName()
        {
            return _textureName;
        }

        public Vector4 GetAdjustedTexturePosition()
        {
            return _texturePosition / Textures.TextureAtlasSize;
        }
    }
}