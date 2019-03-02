using Microsoft.Xna.Framework;

namespace DProject.Type
{
    public class Texture
    {
        private readonly string _assetName;
        private readonly Color _defaultColor;

        public Texture(string assetName, Color defaultColor)
        {
            this._assetName = assetName;
            this._defaultColor = defaultColor;
        }

        public Texture(string assetName) : this(assetName, new Color(1f, 1f, 1f)) { }

        public string GetAssetName()
        {
            return _assetName;
        }

        public Color GetDefaultColor()
        {
            return _defaultColor;
        }
    }
}