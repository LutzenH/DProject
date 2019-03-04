using Microsoft.Xna.Framework;

namespace DProject.Type
{
    public class Prop
    {
        private readonly string _assetName;
        private readonly Vector3 _defaultScale;

        public Prop(string assetName, Vector3 defaultScale)
        {
            _assetName = assetName;
            _defaultScale = defaultScale;
        }

        public Prop(string assetName) : this(assetName, new Vector3(1, 1, 1)) { }

        public string GetAssetName()
        {
            return _assetName;
        }

        public Vector3 GetDefaultScale()
        {
            return _defaultScale;
        }
    }
}