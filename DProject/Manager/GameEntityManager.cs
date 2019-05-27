using DProject.Entity;
using DProject.Entity.Camera;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class GameEntityManager : EntityManager
    {
        private PlayerEntity _playerEntity;
        
        public GameEntityManager()
        {
            _playerEntity = new PlayerEntity(this, 0, 0, 0);
            AddEntity(_playerEntity);
            
            AddCamera(new OrbitCameraEntity(_playerEntity, Quaternion.Identity));
        }
    }
}