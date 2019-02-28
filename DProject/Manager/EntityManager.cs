using System.Collections.Generic;
using DProject.Entity;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class EntityManager
    {
        private List<AbstractEntity> entities;
        private CameraEntity activeCamera;

        public EntityManager()
        {
            activeCamera = new CameraEntity(new Vector3(0f,0f,-1f), new Quaternion(0,0,0,1));
            entities = new List<AbstractEntity>();
             
            entities.Add(activeCamera);
            entities.Add(new PropEntity(Vector3.Zero, "factory"));

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    entities.Add(new PropEntity(new Vector3(1f + x, 0f, -5f + y),"barrel"));
                }
            }
        }

        public List<AbstractEntity> GetEntities()
        {
            return entities;
        }

        public CameraEntity GetActiveCamera()
        {
            return activeCamera;
        }
    }
}