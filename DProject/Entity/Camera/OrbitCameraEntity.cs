using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity.Camera
{
    public class OrbitCameraEntity : CameraEntity
    {
        private const float ScrollSpeed = 1f;
        private const float CameraRotationSpeed = 2f;
        private const float MinimumCameraZoom = 3f;
        private const float MaximumCameraZoom = 20f;

        private const float ToRadians = (float)(Math.PI *2d / 360f);

        private readonly AbstractEntity _entity;

        private Vector3 _cameraOffset;
        private float _distanceFromCamera = 10f;
        private float _cameraOrbitInDegrees;
        
        public OrbitCameraEntity(AbstractEntity entityToOrbit, Quaternion rotation) : base(new Vector3(0,0,0f), rotation)
        {
            _entity = entityToOrbit;
            
            _cameraOffset = new Vector3(0,_distanceFromCamera,_distanceFromCamera);
            
            var entityPosition = _entity.GetPosition();
            Position = entityPosition + _cameraOffset;
        }
        
        public override void Update(GameTime gameTime)
        {
            var entityPosition = _entity.GetPosition();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _cameraOrbitInDegrees -= CameraRotationSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _cameraOrbitInDegrees += CameraRotationSpeed;

            if (Mouse.GetState().ScrollWheelValue < Game1.PreviousMouseState.ScrollWheelValue)
                _distanceFromCamera += ScrollSpeed;
            else if (Mouse.GetState().ScrollWheelValue > Game1.PreviousMouseState.ScrollWheelValue)
                _distanceFromCamera -= ScrollSpeed;

            _distanceFromCamera = MathHelper.Clamp(_distanceFromCamera, MinimumCameraZoom, MaximumCameraZoom);
            
            _cameraOffset = new Vector3(0,_distanceFromCamera,_distanceFromCamera);

            var rotationInRadians = _cameraOrbitInDegrees * ToRadians;
            
            var angleMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, rotationInRadians);
            var newCameraPosition = Vector3.Transform(_cameraOffset, angleMatrix);
            newCameraPosition += entityPosition;
            
            Position = newCameraPosition;
            
            CameraDirection = Vector3.Normalize(entityPosition - Position);

            base.Update(gameTime);
        }
    }
}