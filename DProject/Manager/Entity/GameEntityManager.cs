using DProject.Entity;
using DProject.Entity.Camera;
using Microsoft.Xna.Framework;

namespace DProject.Manager.Entity
{
    public class GameEntityManager : EntityManager
    {
        public GameEntityManager()
        {
            AddEntity(new WaterPlaneEntity(Vector2.Zero, new Vector2(128, 128)));
            AddEntity(new PropEntity(new Vector3(12, WaterPlaneEntity.WaterHeight*2, 5), 9));
            
            AddCamera(new FlyCameraEntity(new Vector3(0, 10, 0), Quaternion.Identity));
        }
    }
}
