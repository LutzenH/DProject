using DProject.Entity.Camera;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class GameEntityManager : EntityManager
    {
        public GameEntityManager()
        {
            AddCamera(new FlyCameraEntity(new Vector3(0f, 10f, 0f), Quaternion.Identity));
        }
    }
}