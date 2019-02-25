using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Entity
{
    public class PropEntity : AbstractEntity, IDrawable
    {
        //Models
        private Model model;
        private readonly string modelPath;

        public PropEntity(Vector3 position, string modelPath) : base(position)
        {
            this.modelPath = modelPath;
        }

        public override void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(modelPath);
        }

        public override void Update(GameTime gameTime)
        {
            //PropEntity does not Update by default.
        }

        public Model getModel()
        {
            return model;
        }
    }
}