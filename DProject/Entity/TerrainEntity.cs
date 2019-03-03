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
        private Texture2D terrainTexture;
        
        public TerrainEntity(Vector3 position, int width, int height, float noiseScale) : base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            HeightMap = new HeightMap(width, height, position.X, position.Z, noiseScale);
        }

        public TerrainEntity(Vector3 position, float[,] heightmap): base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            HeightMap = new HeightMap(heightmap);
        }

        public override void LoadContent(ContentManager content)
        {
            terrainTexture = content.Load<Texture2D>(Textures.texture_atlas_location);
        }

        public void Draw(CameraEntity activeCamera)
        {
            HeightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), terrainTexture);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            HeightMap.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime) {}
    }
}