using System;
using System.Runtime.InteropServices.ComTypes;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DProject.Entity
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private HeightMap HeightMap;
        private float noiseScale;
        
        public TerrainEntity(Vector3 position, int width, int height, float noiseScale) : base(position, Quaternion.Identity, new Vector3(1,5,1))
        {
            this.noiseScale = noiseScale;
            HeightMap = new HeightMap(width, height, position.X, position.Z, noiseScale);
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

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                HeightMap.UpdateTerrain(noiseScale += 1f);
            if(Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                HeightMap.UpdateTerrain(noiseScale -= 1f);
        }
    }
}