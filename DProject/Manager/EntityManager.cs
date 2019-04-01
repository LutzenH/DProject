using System.Collections.Generic;
using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Entity.Interface;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.Manager
{
    public class EntityManager
    {
        private readonly List<AbstractEntity> _entities;
        private readonly List<CameraEntity> _cameraEntities;
        
        public static readonly LinkedList<Message> Messages = new LinkedList<Message>();
        
        private CameraEntity _activeCamera;

        private ChunkLoaderEntity _chunkLoaderEntity;
        private WorldEditorEntity _worldEditorEntity;

        private int _cameraIndex;
        
        public EntityManager()
        {            
            //Entities Lists Initialisation
            _entities = new List<AbstractEntity>();
            _cameraEntities = new List<CameraEntity>
            {
                new FlyCameraEntity(new Vector3(0f, 10f, 0f), Quaternion.Identity),
                new FlyCameraEntity(new Vector3(0f, 0f, 0f), Quaternion.Identity)
            };            
            //Adds all camera's to entities list
            foreach (var cameraEntity in _cameraEntities)
            {
                _entities.Add(cameraEntity);
            }

            _activeCamera = _cameraEntities[0];
            _activeCamera.IsActiveCamera = true;
            
            _chunkLoaderEntity = new ChunkLoaderEntity(this);
            
            _entities.Add(_chunkLoaderEntity);
            _worldEditorEntity = new WorldEditorEntity(this, _chunkLoaderEntity);
            
            _entities.Add(_worldEditorEntity);
            
            _entities.Add(new DebugEntity(_cameraEntities, _chunkLoaderEntity));
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
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && Game1.PreviousKeyboardState.IsKeyDown(Keys.Tab))
                SetNextCamera();
            
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

        public static void AddMessage(Message message)
        {
            Messages.AddLast(message);
        }

        public static Message GetFirstMessage()
        {
            if (Messages.First != null)
            {
                Message message = Messages.First.Value;
                Messages.RemoveFirst();
                return message;
            }

            return null;
        }

        public static int GetMessagesCount()
        {
            return Messages.Count;
        }

        public WorldEditorEntity GetWorldEditorEntity()
        {
            return _worldEditorEntity;
        }

        public ChunkLoaderEntity GetChunkLoaderEntity()
        {
            return _chunkLoaderEntity;
        }

        public static void ClearMessageList()
        {
            Messages.Clear();
        }
    }
}