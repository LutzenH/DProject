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

            AddEntity(new WaterPlaneEntity(Vector2.Zero, new Vector2(128, 128)));
            AddEntity(new PropEntity(new Vector3(12, WaterPlaneEntity.WaterHeight*2, 5), 9));
            
            _worldEditorEntity = new WorldEditorEntity(this);
            AddEntity(_worldEditorEntity);

            _debugEntity = new DebugEntity(this);
            AddEntity(_debugEntity);
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