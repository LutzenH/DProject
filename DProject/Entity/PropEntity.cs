using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;

namespace DProject.Entity
{
    public class PropEntity : AbstractEntity, IDrawable
    {        
        //Models
        private Model _model;
        private readonly string _modelPath;

        public PropEntity(Vector3 position, Quaternion rotation, Vector3 scale, ushort id) : base(position, rotation, scale)
        {
            _modelPath = Props.PropList[id].GetAssetPath();
        }
        
        public PropEntity(Vector3 position, float pitch, float yaw, float roll, Vector3 scale, ushort id) : base(position, pitch, yaw, roll, scale)
        {
            _modelPath = Props.PropList[id].GetAssetPath();
        }

        public PropEntity(Vector3 position, ushort id) : this(position, Quaternion.Identity, Props.PropList[id].GetDefaultScale(), id) { }

        public override void LoadContent(ContentManager content)
        {
            _model = content.Load<Model>(_modelPath);
        }

        public Model GetModel()
        {
            return _model;
        }
        
        public void Draw(CameraEntity activeCamera)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {                
                if (activeCamera.GetBoundingFrustum().Intersects(mesh.BoundingSphere.Transform(GetWorldMatrix())))
                {
                    foreach (var effect1 in mesh.Effects)
                    {
                        var effect = (BasicEffect) effect1;
                        effect.View = activeCamera.GetViewMatrix();
                        effect.World = GetWorldMatrix();
                        effect.Projection = activeCamera.GetProjectMatrix();

                        effect.FogEnabled = true;
                        effect.FogColor = Color.DarkGray.ToVector3();
                        effect.FogStart = 120f;
                        effect.FogEnd = 160f;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}