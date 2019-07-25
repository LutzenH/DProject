using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Entity.Ports;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class GameEntityManager : EntityManager
    {
        private readonly GameTimeEntity _gameTimeEntity;
        
        public GameEntityManager()
        {
            _gameTimeEntity = new GameTimeEntity();
            AddEntity(_gameTimeEntity);
            
            AddCamera(new FlyCameraEntity(new Vector3(0, 10, 0), Quaternion.Identity));
        }
        
        public GameTimeEntity GetGameTimeEntity() => _gameTimeEntity;
    }
}
