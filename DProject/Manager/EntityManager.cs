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
            
            float[,] heightmap = new float[100,100];

            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    entities.Add(new PropEntity(new Vector3(-25 + x, 5, -25 + y),"barrel"));
                }
            }
            
            entities.Add(new TerrainEntity(new Vector3(0, 0, 0), heightmap));
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