using DProject.Game.Interface;
using Microsoft.Xna.Framework;

namespace DProject.Game.Component
{
    public class TransformComponent : IComponent, ITransform, IRotatable, IScalable
    {
        private bool _worldMatrixIsDirty;
        
        private Vector3 _position;
        private Vector3 _scale;
        private Quaternion _rotation;

        private TransformComponent _parent;

        private Matrix _worldMatrix;

        public TransformComponent()
        {
            _position = Vector3.Zero;
            _scale = Vector3.One;
            _rotation = Quaternion.Identity;
            _parent = null;
            _worldMatrixIsDirty = true;
        }

        public Vector3 Position {
            get => _position;
            set
            {
                _position = value;
                _worldMatrixIsDirty = true;
            }
        }
        
        public Quaternion Rotation {
            get => _rotation;
            set
            {
                _rotation = value;
                _worldMatrixIsDirty = true;
            }
        }
        
        public Vector3 Scale {
            get => _scale;
            set
            {
                _scale = value;
                _worldMatrixIsDirty = true;
            }
        }

        public Matrix WorldMatrix {
            get
            {
                if (_worldMatrixIsDirty)
                {
                    _worldMatrix = CalculateWorldMatrix(_position, _scale, _rotation);
                    _worldMatrixIsDirty = false;
                }

                if (_parent == null)
                    return _worldMatrix;
                else
                    return _worldMatrix * _parent.WorldMatrix;
            }
        }

        public TransformComponent Parent {
            get => _parent;
            set
            {
                if (value != this)
                    _parent = value;
            }
        }

        public static Matrix CalculateWorldMatrix(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            return Matrix.CreateFromQuaternion(rotation) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        }
    }
}
