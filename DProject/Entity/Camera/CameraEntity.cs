using System;
using Microsoft.Xna.Framework;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Camera
{
    public abstract class CameraEntity : AbstractEntity, IUpdateable
    {
        private Vector3 _cameraDirection;

        //Matrix
        protected Matrix ProjectMatrix;
        protected Matrix ViewMatrix;

        private const float DefaultNearPlaneDistance = 0.1f;
        private const float DefaultFarPlaneDistance = 180f;
        
        private float _nearPlaneDistance;
        private float _farPlaneDistance;
        
        //BoundingFrustum (used for culling)
        protected BoundingFrustum BoundingFrustum;

        public bool IsActiveCamera = false;

        protected CameraEntity(Vector3 position, Quaternion rotation) : base(position, rotation, new Vector3(1,1,1))
        {
            Position = position;
            CameraDirection = Vector3.Forward;
            CameraDirection.Normalize();
            
            _nearPlaneDistance = DefaultNearPlaneDistance;
            _farPlaneDistance = DefaultFarPlaneDistance;
            
            ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), Game1.ScreenResolutionX/(float)Game1.ScreenResolutionY, _nearPlaneDistance, _farPlaneDistance);
        }
        
        public virtual void Update(GameTime gameTime)
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Position + CameraDirection, Vector3.Up);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), Game1.ScreenResolutionX/(float)Game1.ScreenResolutionY, _nearPlaneDistance, _farPlaneDistance);
            BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectMatrix);
        }

        protected Vector3 CameraDirection
        {
            get => _cameraDirection;
            set
            {
                _cameraDirection = value;

                var args = new CameraDirectionChangedEventArgs
                {
                    CameraDirection = value
                };

                OnDirectionChanged(args);
            }
        }
        
        public BoundingFrustum GetBoundingFrustum()
        {
            return BoundingFrustum;
        }

        public Matrix GetProjectMatrix()
        {
            return ProjectMatrix;
        }
        
        public Matrix GetViewMatrix()
        {
            return ViewMatrix;
        }

        public float GetNearPlaneDistance()
        {
            return _nearPlaneDistance;
        }

        public void SetNearPlaneDistance(float distance)
        {
            _nearPlaneDistance = distance;
        }

        public float GetFarPlaneDistance()
        {
            return _farPlaneDistance;
        }
        
        public void SetFarPlaneDistance(float distance)
        {
            _farPlaneDistance = distance;
        }

        public Matrix GetReflectionViewMatrix()
        {
            var distanceFromWater = 2 * Position.Y - 4 * WaterPlaneEntity.WaterHeight;
            var underWaterPosition = new Vector3(Position.X, Position.Y - distanceFromWater, Position.Z);
            
            return Matrix.CreateLookAt(underWaterPosition, underWaterPosition + new Vector3(CameraDirection.X, -CameraDirection.Y, CameraDirection.Z), Vector3.Up);
        }
        
        public event EventHandler<CameraDirectionChangedEventArgs> DirectionChanged;
        protected void OnDirectionChanged(CameraDirectionChangedEventArgs e)
        {
            var handler = DirectionChanged;
            handler?.Invoke(this, e);
        }
    }
    
    public class CameraDirectionChangedEventArgs : EventArgs
    {
        public Vector3 CameraDirection { get; set; }
    }
}