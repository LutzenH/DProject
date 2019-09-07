using DProject.Game.Component;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class CameraSystem : EntityProcessingSystem
    {
        public static LensComponent ActiveLens;
        
        private ComponentMapper<LensComponent> _lensMapper;
        private ComponentMapper<FlyCameraComponent> _flyCameraMapper;

        public CameraSystem() : base(Aspect.All(typeof(LensComponent), typeof(FlyCameraComponent))) { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _lensMapper = mapperService.GetMapper<LensComponent>();
            _flyCameraMapper = mapperService.GetMapper<FlyCameraComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var lens = _lensMapper.Get(entityId);
            var fly = _flyCameraMapper.Get(entityId);

            if (ActiveLens == null)
                ActiveLens = lens;
            
            var angleX = 0f;
            var angleY = 0f;

            var moveSpeed = (float) ((InputManager.IsInputDown(Input.CameraIncreasedSpeed) ? fly.Speed * 2 : fly.Speed) * gameTime.ElapsedGameTime.TotalSeconds);
        
            var translation = new Vector3(0,0,0);
            
            if (InputManager.IsInputDown(Input.CameraMoveForward))
                translation += lens.Direction * moveSpeed;
            if (InputManager.IsInputDown(Input.CameraMoveBackwards))
                translation -= lens.Direction * moveSpeed;
            if (InputManager.IsInputDown(Input.CameraMoveLeft))
                translation += Vector3.Cross(Vector3.Up, lens.Direction) * moveSpeed;
            if (InputManager.IsInputDown(Input.CameraMoveRight))
                translation -= Vector3.Cross(Vector3.Up, lens.Direction) * moveSpeed;
            if (InputManager.IsInputDown(Input.CameraMoveUp))
                translation += new Vector3(0, moveSpeed, 0);
            if (InputManager.IsInputDown(Input.CameraMoveDown))
                translation -= new Vector3(0, moveSpeed, 0);

            if (!lens.CustomAspectRatio)
                lens.AspectRatio = LensComponent.CalculateAspectRatio(Game1.ScreenResolutionX, Game1.ScreenResolutionY);
                
            lens.Position += translation;

            if (InputManager.IsInputDown(Input.CameraFreeRotation))
            {
                var mousePositionDifference = InputManager.CameraLookVector;
                
                angleX -= mousePositionDifference.X * moveSpeed * 3;
                angleY += mousePositionDifference.Y * moveSpeed * 3;
            }
            else
            {
                if (InputManager.IsInputDown(Input.CameraLookLeft))
                    angleX -= moveSpeed * 6;
                if (InputManager.IsInputDown(Input.CameraLookRight))
                    angleX += moveSpeed * 6;
                if (InputManager.IsInputDown(Input.CameraLookUp))
                    angleY += moveSpeed * 6;
                if (InputManager.IsInputDown(Input.CameraLookDown))
                    angleY -= moveSpeed * 6;
            }

            var cameraUpPerpendicular = Vector3.Cross(Vector3.Up, lens.Direction);
            cameraUpPerpendicular.Normalize();
        
            var tempDirection = Vector3.Transform(lens.Direction, Matrix.CreateFromAxisAngle(cameraUpPerpendicular, (-MathHelper.PiOver4 / 150) * angleY));
            tempDirection = Vector3.Transform(tempDirection, Matrix.CreateFromAxisAngle(Vector3.Up,(-MathHelper.PiOver4 / 150) * angleX));
            tempDirection.Normalize();

            lens.Direction = tempDirection;
        }
    }
}
