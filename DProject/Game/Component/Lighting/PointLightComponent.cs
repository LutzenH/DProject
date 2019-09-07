using Microsoft.Xna.Framework;

namespace DProject.Game.Component.Lighting
{
    public class PointLightComponent
    {
        private bool _worldMatrixIsDirty;

        private float _radius;
        private Vector3 _position;
        
        private Matrix _worldMatrix;

        public PointLightComponent()
        {
            _worldMatrixIsDirty = true;
        }

        public Vector3 Color { get; set; }
        public float Intensity { get; set; }
        
        public float Radius {
            get => _radius;
            set
            {
                _radius = value;
                _worldMatrixIsDirty = true;
            }
        }
        
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                _worldMatrixIsDirty = true;
            }
        }
        
        public Matrix WorldMatrix {
            get
            {
                if (_worldMatrixIsDirty)
                {
                    _worldMatrix = Matrix.CreateScale(new Vector3(_radius)) * Matrix.CreateTranslation(_position);
                    _worldMatrixIsDirty = false;
                }

                return _worldMatrix;
            }
        }
    }
}
