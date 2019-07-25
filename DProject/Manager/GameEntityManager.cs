using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Entity.Ports;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class GameEntityManager : EntityManager
    {
        public GameEntityManager()
        {
            AddEntity(new GameTimeEntity());
            AddCamera(new FlyCameraEntity(new Vector3(0, 10, 0), Quaternion.Identity));
        }
    }
}