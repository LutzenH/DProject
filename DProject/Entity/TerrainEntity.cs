using System;
using System.Runtime.InteropServices.ComTypes;
using DProject.List;
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

        private Texture2D grassTexture;
        
        public TerrainEntity(Vector3 position, int width, int height, float noiseScale) : base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            this.noiseScale = noiseScale;
            HeightMap = new HeightMap(width, height, position.X, position.Z, noiseScale);
        }

        public override void LoadContent(ContentManager content)
        {
            grassTexture = content.Load<Texture2D>(Textures.TextureList["savanna_grass"].GetAssetName());
        }

        public void Draw(CameraEntity activeCamera)
        {
            HeightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), getWorldMatrix(), grassTexture);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            HeightMap.Initialize(graphicsDevice);
        }

        public float[,] GetHeightData()
        {
            return HeightMap.GetHeightData();
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