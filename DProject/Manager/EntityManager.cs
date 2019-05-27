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
        private readonly List<AbstractEntity> _entities;
        private readonly List<CameraEntity> _cameraEntities;
        
        private readonly ChunkLoaderEntity _chunkLoaderEntity;
        private readonly PointerEntity _pointerEntity;
        private CameraEntity _activeCamera;

        private int _cameraIndex;

        protected EntityManager()
        {
            _entities = new List<AbstractEntity>();
            _cameraEntities = new List<CameraEntity>();
            
            _chunkLoaderEntity = new ChunkLoaderEntity(this);
            AddEntity(_chunkLoaderEntity);

            _pointerEntity = new PointerEntity(this);
            AddEntity(_pointerEntity);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            foreach (var entity in _entities)
            {
                if (entity is IInitialize initializeEntity)
                    initializeEntity.Initialize(graphicsDevice);
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach (var entity in _entities)
            {
                entity.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                if (entity is Entity.Interface.IUpdateable updateEntity)
                    updateEntity.Update(gameTime);
            }
        }

        public void Draw()
        {
            foreach (var entity in _entities)
            {
                if (entity is Entity.Interface.IDrawable drawableEntity)
                    drawableEntity.Draw(GetActiveCamera());
            }
        }

        public List<AbstractEntity> GetEntities()
        {
            return _entities;
        }

        public void AddEntity(AbstractEntity entity)
        {
            _entities.Add(entity);
        }

        public List<CameraEntity> GetCameraEntities()
        {
            return _cameraEntities;
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

        public PointerEntity GetPointerEntity()
        {
            return _pointerEntity;
        }
    }
}