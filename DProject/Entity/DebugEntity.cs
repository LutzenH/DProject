using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Entity
{
    public class DebugEntity : AbstractEntity, IInitialize, IUpdateable, IDrawable
    {
        private PropEntity[] cameraProps;
        private List<CameraEntity> CameraEntities;
                
        public DebugEntity(List<CameraEntity> cameraEntities) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            cameraProps = new PropEntity[cameraEntities.Count];
            this.CameraEntities = cameraEntities;

            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i] = new PropEntity(CameraEntities[i].GetPosition(), Quaternion.Identity, cameraEntities[i].GetScale(), "camera");
            }
        }

        public override void LoadContent(ContentManager content)
        {
            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i].LoadContent(content);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice) { }

        public void Update(GameTime gameTime)
        {            
            for (int i = 0; i < cameraProps.Length; i++)
            {
                Vector3 cameraDirection = CameraEntities[i].GetCameraDirection();

                float yaw = (float) Math.Asin(cameraDirection.X / Math.Cos(Math.Asin(cameraDirection.Y)));
                float pitch = (float) Math.Asin(cameraDirection.Y);
          
                cameraProps[i].SetPosition(CameraEntities[i].GetPosition());
                cameraProps[i].SetRotation(-pitch, yaw+ 1.5f, 0f);
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i].Draw(activeCamera);
            }
        }
    }
}