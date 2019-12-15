using Microsoft.Xna.Framework;

namespace DProject.Game.Component.Lighting
{
    public class PointLightComponent : IComponent
    {
        private bool _worldMatrixIsDirty;

        private float _radius;
        private Vector3 _position;
        
        private TransformComponent _parent;
        
        private Matrix _worldMatrix;

        public PointLightComponent()
        {
            _worldMatrixIsDirty = true;
        }

        public Color Color { get; set; }
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

        public Vector3 WorldPosition => Parent == null ? _position : _position + Parent.Position;

        public TransformComponent Parent {
            get => _parent;
            set => _parent = value;
        }
        
        public Matrix WorldMatrix {
            get
            {
                if (_worldMatrixIsDirty)
                {
                    _worldMatrix = Matrix.CreateScale(new Vector3(_radius*2)) * Matrix.CreateTranslation(_position);
                    _worldMatrixIsDirty = false;
                }

                if (_parent == null)
                    return _worldMatrix;
                else
                    return _worldMatrix * _parent.WorldMatrix;
            }
        }
    }
}
