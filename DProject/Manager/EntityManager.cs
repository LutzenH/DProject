using System.Collections.Generic;
using DProject.Entity;
using Microsoft.Xna.Framework;

namespace DProject.Manager
{
    public class EntityManager
    {
        private readonly List<AbstractEntity> _entities;
        private readonly List<CameraEntity> _cameraEntities;
        
        private CameraEntity _activeCamera;

        private int _cameraIndex;
        
        public EntityManager()
        {
            //Entities Lists Initialisation
            _entities = new List<AbstractEntity>();
            _cameraEntities = new List<CameraEntity>
            {
                new CameraEntity(new Vector3(0f, 0f, -1f), Quaternion.Identity),
                new CameraEntity(new Vector3(0f, 0f, -5f), Quaternion.Identity)
            };

            _entities.Add(new PropEntity(new Vector3(32, 5,32), "barrel"));

            //Adds all camera's to entities list
            foreach (var cameraEntity in _cameraEntities)
            {
                _entities.Add(cameraEntity);
            }

            _activeCamera = _cameraEntities[0];
            _activeCamera.IsActiveCamera = true;
            
            var chunkLoaderEntity = new ChunkLoaderEntity(this);
            
            _entities.Add(chunkLoaderEntity);
            _entities.Add(new PointerEntity(this, chunkLoaderEntity));
            _entities.Add(new DebugEntity(_cameraEntities));
        }

        public List<AbstractEntity> GetEntities()
        {
            return _entities;
        }

        public CameraEntity GetActiveCamera()
        {
            return _activeCamera;
        }

        public void SetActiveCamera(int index)
        {
            _activeCamera.IsActiveCamera = false;
            _activeCamera = _cameraEntities[index];
            _activeCamera.IsActiveCamera = true;
        }

        public void SetNextCamera()
        {
            _cameraIndex++;
            _cameraIndex %= _cameraEntities.Count;

            _activeCamera.IsActiveCamera = false;
            
            _activeCamera = _cameraEntities[_cameraIndex];
            _activeCamera.IsActiveCamera = true;
        }
    }
}