using System.Collections.Generic;
using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Entity.Debug;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.Manager
{
    public class EditorEntityManager : EntityManager
    {
        public static readonly LinkedList<Message> Messages = new LinkedList<Message>();
        
        private readonly WorldEditorEntity _worldEditorEntity;
        private readonly DebugEntity _debugEntity;

        public EditorEntityManager()
        {            
            AddCamera(new FlyCameraEntity(new Vector3(0f, 10f, 0f), Quaternion.Identity));
            AddCamera(new FlyCameraEntity(new Vector3(0f, 0f, 0f), Quaternion.Identity));

            _worldEditorEntity = new WorldEditorEntity(this);
            AddEntity(_worldEditorEntity);

            _debugEntity = new DebugEntity(this);
            AddEntity(_debugEntity);
            
            AddEntity(new PropEntity(new Vector3(0, 10, 0), 18));
            AddEntity(new PropEntity(new Vector3(1.5f, 10, 0), 26));
            AddEntity(new PropEntity(new Vector3(3, 10, 0), 19));
            AddEntity(new PropEntity(new Vector3(5, 10, 0), 20));
            AddEntity(new PropEntity(new Vector3(7, 10, 0), 21));
            AddEntity(new PropEntity(new Vector3(9, 10, 0), 22));
            AddEntity(new PropEntity(new Vector3(11, 10, 0), 23));
            AddEntity(new PropEntity(new Vector3(13, 10, 0), 24));
            AddEntity(new PropEntity(new Vector3(13, 10, 2), 25));
            AddEntity(new PropEntity(new Vector3(3, 10, 2), 27));
            AddEntity(new PropEntity(new Vector3(5, 10, 2), 28));
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
                var message = Messages.First.Value;
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