using System;
using System.Runtime.InteropServices.ComTypes;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Entity
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize
    {
        private HeightMap HeightMap;
        
        public TerrainEntity(Vector3 position, int width, int height, float noiseScale) : base(position, Quaternion.Identity, new Vector3(1,5,1))
        {
            HeightMap = new HeightMap(width, height, noiseScale);
        }

        public override void LoadContent(ContentManager content) { }

        public void Draw(CameraEntity activeCamera)
        {
            HeightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), getWorldMatrix());
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            HeightMap.Initialize(graphicsDevice);
        }
    }
}