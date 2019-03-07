using System;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class PointerEntity : AbstractEntity, IInitialize,IUpdateable, IDrawable
    {
        private readonly AxisEntity _axisEntity;
        private readonly PropEntity _propEntity;
        
        private readonly EntityManager _entityManager;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;
        private GraphicsDevice _graphicsDevice;

        public PointerEntity(EntityManager entityManager, ChunkLoaderEntity chunkLoaderEntity) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _axisEntity = new AxisEntity(Vector3.Zero);
            _propEntity = new PropEntity(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1), "plane");
            _entityManager = entityManager;
            _chunkLoaderEntity = chunkLoaderEntity;
        }

        public override void LoadContent(ContentManager content)
        {
            _propEntity.LoadContent(content);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _axisEntity.Initialize(graphicsDevice);

            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            
            Ray ray = Game1.CalculateRay(mouseLocation, _entityManager.GetActiveCamera().GetViewMatrix(),
                _entityManager.GetActiveCamera().GetProjectMatrix(), _graphicsDevice.Viewport);

            Vector3 position = Vector3.Zero;
            if (ray.Direction.Y != 0)
            {
                Vector3 tempPosition = ray.Position - ray.Direction * (ray.Position.Y / ray.Direction.Y);
                position = tempPosition;

                float y = _chunkLoaderEntity.GetHeightFromPosition(new Vector2(tempPosition.X, tempPosition.Z)) ?? 0f;

                position = new Vector3((int)Math.Round(position.X), y, (int)Math.Round(position.Z));
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector2 localChunkPosition = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));

                int x = (int)localChunkPosition.X;
                int y = (int) localChunkPosition.Y;

                _chunkLoaderEntity.GetChunk(position).ChunkStatus = ChunkStatus.Changed;
                _chunkLoaderEntity.GetChunk(position).ChangeTileHeight(0f, x, y);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Vector2 localChunkPosition = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));

                int x = (int)localChunkPosition.X;
                int y = (int) localChunkPosition.Y;

                _chunkLoaderEntity.GetChunk(position).ChunkStatus = ChunkStatus.Changed;
                _chunkLoaderEntity.GetChunk(position).ChangeTexture("metal", x, y);
            }

            _axisEntity.SetPosition(new Vector3(position.X, 0, position.Z));
            _propEntity.SetPosition(position);
            
            foreach (var entity in _entityManager.GetEntities())
            {
                if (entity is PropEntity propEntity)
                {
                    if (Game1.Intersects(
                        new Vector2(mouseLocation.X, mouseLocation.Y),
                        propEntity.GetModel(),
                        propEntity.GetWorldMatrix(),
                        _entityManager.GetActiveCamera().GetViewMatrix(),
                        _entityManager.GetActiveCamera().GetProjectMatrix(),
                        _graphicsDevice.Viewport))
                    {
                        _axisEntity.SetPosition(propEntity.GetPosition());
                    }
                }
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            _axisEntity.Draw(activeCamera);
            _propEntity.Draw(activeCamera);
        }
    }
}