using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DProject.Entity
{
    public abstract class AbstractEntity
    {
        protected Vector3 Position;

        protected AbstractEntity(Vector3 position)
        {
            this.Position = position;
        }

        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime);
    }
}