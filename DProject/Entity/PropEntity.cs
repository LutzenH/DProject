using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity
{
    public class PropEntity : AbstractEntity, IDrawable
    {
        private float yaw = 0;
        //Models
        private Model model;
        private readonly string modelPath;

        public PropEntity(Vector3 position, Quaternion rotation, Vector3 scale, string name) : base(position, rotation, scale)
        {
            this.modelPath = Props.PropList[name].GetAssetName();
        }
        
        public PropEntity(Vector3 position, float pitch, float yaw, float roll, Vector3 scale, string name) : base(position, pitch, yaw, roll, scale)
        {
            this.modelPath = Props.PropList[name].GetAssetName();
        }

        public PropEntity(Vector3 position, string name) : this(position, Quaternion.Identity, Props.PropList[name].GetDefaultScale(), name) { }

        public override void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(modelPath);
        }

        public Model getModel()
        {
            return model;
        }
        
        public void Draw(CameraEntity activeCamera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {                            
                if (activeCamera.GetBoundingFrustum().Intersects(mesh.BoundingSphere.Transform(getWorldMatrix())))
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.View = activeCamera.GetViewMatrix();
                        effect.World = getWorldMatrix();
                        effect.Projection = activeCamera.GetProjectMatrix();
                    }

                    mesh.Draw();
                }
            }
        }
    }
}