using Microsoft.Xna.Framework;

namespace DProject.Game.Component
{
    public class LensComponent
    {
        private bool _projectionIsDirty;
        private bool _viewIsDirty;
        private bool _reflectionViewIsDirty;
        private bool _boundingFrustumIsDirty;
        private bool _reflectionBoundingFrustumIsDirty;

        private Vector3 _position;
        private Vector3 _direction;
        
        private float _nearPlaneDistance;
        private float _farPlaneDistance;
        private float _fieldOfView;
        private float _reflectionPlaneHeight;

        private float _aspectRatio;
        
        private Matrix _projectMatrix;
        private Matrix _viewMatrix;
        private Matrix _reflectionViewMatrix;

        private BoundingFrustum _boundingFrustum;
        private BoundingFrustum _reflectionBoundingFrustum;
        
        public bool CustomAspectRatio { get; set; }

        public LensComponent()
        {
            _nearPlaneDistance = 1f;
            _farPlaneDistance = 6000f;
            _fieldOfView = 80f;
            _aspectRatio = 1/2f;

            CustomAspectRatio = false;
            
            _position = Vector3.Zero;
            _direction = Vector3.Forward;
            
            _projectionIsDirty = true;
            _viewIsDirty = true;
            _reflectionViewIsDirty = true;
            _boundingFrustumIsDirty = true;
            _reflectionBoundingFrustumIsDirty = true;
        }

        #region Projection

        public float NearPlaneDistance {
            get => _nearPlaneDistance;
            set
            {
                _nearPlaneDistance = value;
                _projectionIsDirty = true;
            }
        }
        
        public float FarPlaneDistance {
            get => _farPlaneDistance;
            set
            {
                _farPlaneDistance = value;
                _projectionIsDirty = true;
            }
        }
        
        public float FieldOfView {
            get => _fieldOfView;
            set
            {
                _fieldOfView = value;
                _projectionIsDirty = true;
            }
        }

        public Matrix Projection
        {
            get
            {
                if (_projectionIsDirty)
                {
                    _projectMatrix = CalculateProjectionMatrix(_fieldOfView, _aspectRatio, _nearPlaneDistance, _farPlaneDistance);
                    _projectionIsDirty = false;
                    _boundingFrustumIsDirty = true;
                    _reflectionBoundingFrustumIsDirty = true;
                }

                return _projectMatrix;
            }
        }

        private static Matrix CalculateProjectionMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                _projectionIsDirty = true;
            }
        }
        
        public static float CalculateAspectRatio(int screenResolutionX, int screenResolutionY)
        {
            return screenResolutionX / (float) screenResolutionY;
        }

        #endregion

        #region View

        public Vector3 Position {
            get => _position;
            set
            {
                _position = value;
                _viewIsDirty = true;
                _reflectionViewIsDirty = true;
            }
        }

        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _viewIsDirty = true;
                _reflectionViewIsDirty = true;
            }
        }

        public Matrix View {
            get
            {
                if (_viewIsDirty)
                {
                    _viewMatrix = CalculateViewMatrix(_position, _direction);
                    _viewIsDirty = false;
                    _boundingFrustumIsDirty = true;
                }

                return _viewMatrix;
            }
        }

        public float ReflectionPlaneHeight
        {
            get => _reflectionPlaneHeight;
            set
            {
                _reflectionPlaneHeight = value;
                _reflectionViewIsDirty = true;
            }
        }

        public Matrix ReflectionView
        {
            get
            {
                if (_reflectionViewIsDirty)
                {
                    _reflectionViewMatrix = CalculateReflectionViewMatrix(_position, _direction, _reflectionPlaneHeight);
                    _reflectionViewIsDirty = false;
                    _reflectionBoundingFrustumIsDirty = true;
                }

                return _reflectionViewMatrix;
            }
        }

        private static Matrix CalculateViewMatrix(Vector3 position, Vector3 direction)
        {
            direction.Normalize();
            return Matrix.CreateLookAt(position, position + direction, Vector3.Up);
        }
        
        private static Matrix CalculateReflectionViewMatrix(Vector3 position, Vector3 direction, float planeHeight)
        {
            var distanceFromWater = 2 * (position.Y - planeHeight);
            var underWaterPosition = new Vector3(position.X, position.Y - distanceFromWater, position.Z);
            
            return Matrix.CreateLookAt(underWaterPosition, underWaterPosition + new Vector3(direction.X, -direction.Y, direction.Z), Vector3.Up);
        }

        #endregion

        #region BoundingFrustum
        
        public BoundingFrustum BoundingFrustum
        {
            get
            {
                if (_boundingFrustumIsDirty)
                {
                    _boundingFrustum = CalculateBoundingFrustum(_viewMatrix, _projectMatrix);
                    _boundingFrustumIsDirty = false;
                }

                return _boundingFrustum;
            }
        }
        
        public BoundingFrustum ReflectionBoundingFrustum
        {
            get
            {
                if (_reflectionBoundingFrustumIsDirty)
                {
                    _reflectionBoundingFrustum = CalculateBoundingFrustum(_reflectionViewMatrix, _projectMatrix);
                    _reflectionBoundingFrustumIsDirty = false;
                }

                return _reflectionBoundingFrustum;
            }
        }

        private static BoundingFrustum CalculateBoundingFrustum(Matrix viewMatrix, Matrix projectMatrix)
        {
            return new BoundingFrustum(viewMatrix * projectMatrix);
        }

        #endregion
    }
}
