using DProject.Entity.Interface;
using DProject.List;
using DProject.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class TerrainEntity : AbstractEntity, IDrawable, IInitialize, IUpdateable
    {
        private readonly HeightMap _heightMap;
        private Texture2D _terrainTexture;

        private readonly int _chunkPositionX;
        private readonly int _chunkPositionY;
        
        public TerrainEntity(Vector3 position, int width, int height, float noiseScale) : base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            _heightMap = new HeightMap(width, height, position.X, position.Z, noiseScale);
        }

        public TerrainEntity(int x, int y, int size, float noiseScale) : base(new Vector3(x*size, 0, y*size), Quaternion.Identity, new Vector3(1,1,1))
        {
            _chunkPositionX = x;
            _chunkPositionY = y;
            _heightMap = new HeightMap(size, size,  x*size, y*size, noiseScale);
        }

        public TerrainEntity(Vector3 position, float[,] heightmap): base(position, Quaternion.Identity, new Vector3(1,1,1))
        {
            _heightMap = new HeightMap(heightmap);
        }

        public override void LoadContent(ContentManager content)
        {
            _terrainTexture = content.Load<Texture2D>(Textures.TextureAtlasLocation);
        }

        public void Draw(CameraEntity activeCamera)
        {
            _heightMap.Draw(activeCamera.GetProjectMatrix(),activeCamera.GetViewMatrix(), GetWorldMatrix(), _terrainTexture);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _heightMap.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime) {}

        public int GetChunkX()
        {
            return _chunkPositionX;
        }

        public int GetChunkY()
        {
            return _chunkPositionY;
        }
    }
}