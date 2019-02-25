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
            activeCamera = new CameraEntity(new Vector3(0f,0f,-10f));
            entities = new List<AbstractEntity>();
            
            entities.Add(activeCamera);
            entities.Add(new PropEntity(Vector3.Zero, "models/factory"));
            entities.Add(new PropEntity(new Vector3(0f, 0.8f, -2.5f), "models/cube"));
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