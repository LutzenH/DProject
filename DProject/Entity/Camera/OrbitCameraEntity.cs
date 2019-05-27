using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity.Camera
{
    public class OrbitCameraEntity : CameraEntity
    {
        private readonly AbstractEntity _entity;

        private Vector3 _cameraOffset;
        private float _distanceFromCamera = 10f;
        float cameraOrbitInDegrees = 15f;

        public OrbitCameraEntity(AbstractEntity entityToOrbit, Quaternion rotation) : base(new Vector3(0,0,0f), rotation)
        {
            _entity = entityToOrbit;
            
            _cameraOffset = new Vector3(0,10,_distanceFromCamera);
            
            var entityPosition = _entity.GetPosition();
            Position = entityPosition + _cameraOffset;
        }

        public override void LoadContent(ContentManager content) { }

        public override void Update(GameTime gameTime)
        {
            var entityPosition = _entity.GetPosition();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraOrbitInDegrees -= 2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraOrbitInDegrees += 2f;

            const float toRadians = (float)(Math.PI *2d / 360f);
            var rotationInRadians = cameraOrbitInDegrees * toRadians;
            
            var angleMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, rotationInRadians);
            var newCameraPosition = Vector3.Transform(_cameraOffset, angleMatrix);
            newCameraPosition += entityPosition;
            
            Position = newCameraPosition;
            
            CameraDirection = Vector3.Normalize(entityPosition - Position);

            base.Update(gameTime);
        }
    }
}