using DProject.Entity.Camera;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager;
using DProject.Manager.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class PlayerEntity : AbstractAwareEntity, IUpdateable, IDrawable, ILoadContent
    {
        private PropEntity _playerModel;
        private readonly PointerEntity _pointerEntity;
        
        public PlayerEntity(EntityManager entityManager, int x, int y) : base(entityManager, new Vector3(x, 0, y), Quaternion.Identity, new Vector3(1,1,1))
        {
            _playerModel = new PropEntity(Position, Quaternion.Identity, Scale, Props.GetPropIdFromName("human"));

            _pointerEntity = EntityManager.GetPointerEntity();
        }

        public void LoadContent(ContentManager content)
        {
            _playerModel.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                Position = _pointerEntity.GetGridPosition();
                _playerModel.SetPosition(Position);
            }
        }

        public void Draw(CameraEntity activeCamera, ShaderManager shaderManager)
        {
            _playerModel.Draw(activeCamera, shaderManager);
        }
    }
}