using DProject.Entity;
using System.Collections.Generic;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.Manager
{
    public abstract class EntityManager
    {
        protected readonly List<AbstractEntity> _entities;
        protected readonly List<CameraEntity> _cameraEntities;
        
        protected ChunkLoaderEntity _chunkLoaderEntity;
        protected CameraEntity _activeCamera;

        private int _cameraIndex;

        protected EntityManager()
        {
            _entities = new List<AbstractEntity>();
            _cameraEntities = new List<CameraEntity>();
            
            _chunkLoaderEntity = new ChunkLoaderEntity(this);
            _entities.Add(_chunkLoaderEntity);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            foreach (AbstractEntity entity in _entities)
            {
                if (entity is IInitialize initializeEntity)
                    initializeEntity.Initialize(graphicsDevice);
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach (AbstractEntity entity in _entities)
            {
                entity.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (AbstractEntity entity in _entities)
            {
                if (entity is Entity.Interface.IUpdateable updateEntity)
                    updateEntity.Update(gameTime);
            }
        }

        public void Draw()
        {
            foreach (AbstractEntity entity in _entities)
            {
                if (entity is Entity.Interface.IDrawable drawableEntity)
                    drawableEntity.Draw(GetActiveCamera());
            }
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
            if(_activeCamera != null)
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

        protected void AddCamera(CameraEntity cameraEntity)
        {
            var cameraEntitiesIsEmpty = _cameraEntities.Count == 0;

            _cameraEntities.Add(cameraEntity);
            
            if(cameraEntitiesIsEmpty)
                SetActiveCamera(0);
            
            _entities.Add(cameraEntity);
        }
        
        public ChunkLoaderEntity GetChunkLoaderEntity()
        {
            return _chunkLoaderEntity;
        }
    }
}