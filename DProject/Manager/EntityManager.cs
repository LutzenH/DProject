using System.Collections.Generic;
using DProject.Entity;
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
        
        private readonly LinkedList<Message> _messages;
        
        private CameraEntity _activeCamera;

        private int _cameraIndex;
        
        public EntityManager()
        {
            //Entities Lists Initialisation
            _entities = new List<AbstractEntity>();
            _cameraEntities = new List<CameraEntity>
            {
                new CameraEntity(new Vector3(0f, 0f, -1f), Quaternion.Identity),
                new CameraEntity(new Vector3(0f, 500f, -1f), Quaternion.Identity)
            };

            _messages = new LinkedList<Message>();
            
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
            _entities.Add(new DebugEntity(_cameraEntities, chunkLoaderEntity));
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

        public void AddMessage(Message message)
        {
            _messages.AddLast(message);
        }

        public Message GetFirstMessage()
        {
            if (_messages.First != null)
            {
                Message message = _messages.First.Value;
                _messages.RemoveFirst();
                return message;
            }

            return null;
        }

        public int GetMessagesCount()
        {
            return _messages.Count;
        }

        public void ClearMessageList()
        {
            _messages.Clear();
        }
    }
}