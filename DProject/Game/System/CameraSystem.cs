using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

            var moveSpeed = (float) ((Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? fly.Speed * 2 : fly.Speed) * gameTime.ElapsedGameTime.TotalSeconds);
        
            var translation = new Vector3(0,0,0);
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                translation += lens.Direction * moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                translation -= lens.Direction * moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                translation += Vector3.Cross(Vector3.Up, lens.Direction) * moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                translation -= Vector3.Cross(Vector3.Up, lens.Direction) * moveSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                translation += new Vector3(0, moveSpeed, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                translation -= new Vector3(0, moveSpeed, 0);

            lens.Position += translation;

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                var mousePositionDifference = Game1.PreviousMouseState.Position - Mouse.GetState().Position;
                
                angleX -= mousePositionDifference.X * moveSpeed * 3;
                angleY += mousePositionDifference.Y * moveSpeed * 3;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    angleX -= moveSpeed * 6;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    angleX += moveSpeed * 6;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    angleY += moveSpeed * 6;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
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
