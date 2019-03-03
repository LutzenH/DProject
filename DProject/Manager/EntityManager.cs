using System.Collections.Generic;
using DProject.Entity;
using DProject.Type;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class EntityManager
    {
        private List<AbstractEntity> entities;
        private List<CameraEntity> cameraEntities;
        
        private CameraEntity activeCamera;

        private int cameraIndex = 0;
        
        public EntityManager()
        {
            //Entities Lists Initialisation
            entities = new List<AbstractEntity>();
            cameraEntities = new List<CameraEntity>
            {
                new CameraEntity(new Vector3(0f, 0f, -1f), Quaternion.Identity),
                new CameraEntity(new Vector3(0f, 0f, -5f), Quaternion.Identity)
            };

            entities.Add(new PropEntity(new Vector3(32, 5,32), "barrel"));

            //Adds all camera's to entities list
            foreach (var cameraEntity in cameraEntities)
            {
                entities.Add(cameraEntity);
            }

            activeCamera = cameraEntities[0];
            activeCamera.IsActiveCamera = true;
            
            entities.Add(new ChunkLoaderEntity(this));
            entities.Add(new PointerEntity(this));        
            entities.Add(new DebugEntity(cameraEntities));
        }

        public List<AbstractEntity> GetEntities()
        {
            return entities;
        }

        public CameraEntity GetActiveCamera()
        {
            return activeCamera;
        }

        public void SetActiveCamera(int index)
        {
            activeCamera.IsActiveCamera = false;
            activeCamera = cameraEntities[index];
            activeCamera.IsActiveCamera = true;
        }

        public void SetNextCamera()
        {
            cameraIndex++;
            cameraIndex %= cameraEntities.Count;

            activeCamera.IsActiveCamera = false;
            
            activeCamera = cameraEntities[cameraIndex];
            activeCamera.IsActiveCamera = true;
        }
    }
}