using DProject.Entity;
using DProject.Entity.Camera;
using Microsoft.Xna.Framework;

namespace DProject.Manager.Entity
{
    public class GameEntityManager : EntityManager
    {
        public GameEntityManager()
        {
            AddEntity(new WaterPlaneEntity(Vector2.Zero, new Vector2(512, 512)));
            AddEntity(new PropEntity(new Vector3(100, WaterPlaneEntity.WaterHeight*2, 34), 9));
            AddEntity(new PropEntity(new Vector3(50, WaterPlaneEntity.WaterHeight*2 - 0.2f, 12), 8));
            
            AddCamera(new FlyCameraEntity(new Vector3(0, 10, 0), Quaternion.Identity));
        }
    }
}
