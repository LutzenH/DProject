using System.Collections.Generic;
using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.Manager
{
    public class EditorEntityManager : EntityManager
    {
        public static readonly LinkedList<Message> Messages = new LinkedList<Message>();
        
        private WorldEditorEntity _worldEditorEntity;
        private DebugEntity _debugEntity;

        public EditorEntityManager()
        {            
            AddCamera(new FlyCameraEntity(new Vector3(0f, 10f, 0f), Quaternion.Identity));
            AddCamera(new FlyCameraEntity(new Vector3(0f, 0f, 0f), Quaternion.Identity));
            
            _worldEditorEntity = new WorldEditorEntity(this, _chunkLoaderEntity);
            _entities.Add(_worldEditorEntity);

            _debugEntity = new DebugEntity(_cameraEntities, _chunkLoaderEntity);
            _entities.Add(_debugEntity);
        }

        public static void AddMessage(Message message)
        {
            Messages.AddLast(message);
        }

        public new void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && Game1.PreviousKeyboardState.IsKeyDown(Keys.Tab))
                SetNextCamera();
            
            base.Update(gameTime);
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

        public WorldEditorEntity GetWorldEditorEntity()
        {
            return _worldEditorEntity;
        }
    }
}