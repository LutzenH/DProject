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
            cameraEntities = new List<CameraEntity>();

            cameraEntities.Add(new CameraEntity(new Vector3(0f, 0f, -1f), Quaternion.Identity));
            cameraEntities.Add(new CameraEntity(new Vector3(0f, 0f, -5f), Quaternion.Identity));
                                    
            int size = 64;
            
            float[,] heightmap = Noise.GenerateNoiseMap(size, size, 0, 0, 50f);

            entities.Add(new PropEntity(new Vector3(32, heightmap[32,32],32), "barrel"));
            
            entities.Add(new TerrainEntity(new Vector3(-1 * size, 0, -1 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(0 * size, 0, -1 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(1 * size, 0, -1 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(-1 * size, 0, 0 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(0 * size, 0, 0 * size), heightmap));
            entities.Add(new TerrainEntity(new Vector3(1 * size, 0, 0 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(-1 * size, 0, 1 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(0 * size, 0, 1 * size), size,size, 50f));
            entities.Add(new TerrainEntity(new Vector3(1 * size, 0, 1 * size), size,size, 50f));

            //Adds all camera's to entities list
            foreach (var cameraEntity in cameraEntities)
            {
                entities.Add(cameraEntity);
            }

            activeCamera = cameraEntities[0];
            activeCamera.IsActiveCamera = true;
            
            entities.Add(new PointerEntity(this, heightmap));            
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