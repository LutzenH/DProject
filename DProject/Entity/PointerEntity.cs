using System;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Manager.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class PointerEntity : AbstractAwareEntity, IUpdateable, IInitialize
    {
        private GraphicsDevice _graphicsDevice;
        
        private Vector3 _gridPosition;
        private TerrainEntity.TileCorner _selectedCorner;
        
        public PointerEntity(EntityManager entityManager) : base(entityManager, Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1)) { }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        
        public void Update(GameTime gameTime)
        {
            var mouseLocation = Game1.GetMousePosition();

            var position = CalculatePrecisePosition(mouseLocation, EntityManager.GetActiveCamera(), _graphicsDevice, EntityManager.GetChunkLoaderEntity());

            if (position != null)
            {
                Position = (Vector3) position;
                _gridPosition = CalculatePosition(Position);
                _selectedCorner = CalculateCorner(Position);
            }
        }
        
        #region Calculations
        
        private static TerrainEntity.TileCorner CalculateCorner(Vector3 precisePosition)
        {            
            var restX = Math.Abs(precisePosition.X % 1f);
            var restY = Math.Abs(precisePosition.Z % 1f);

            if (precisePosition.X < 0f)
                restX = 1-restX;

            if (precisePosition.Z < 0f)
                restY = 1-restY;

            if (restX > 0.5f)
                return restY > 0.5f ? TerrainEntity.TileCorner.TopLeft : TerrainEntity.TileCorner.BottomLeft;
            else
                return restY > 0.5f ? TerrainEntity.TileCorner.TopRight : TerrainEntity.TileCorner.BottomRight;
        }

        private static Vector3 CalculatePosition(Vector3 precisePosition)
        {
            return new Vector3((int)Math.Round(precisePosition.X), precisePosition.Y, (int)Math.Round(precisePosition.Z));
        }

        private static Vector3? CalculatePrecisePosition(Vector2 mouseLocation, CameraEntity camera,
            GraphicsDevice graphicsDevice, ChunkLoaderEntity chunkLoaderEntity)
        {
            Ray ray = CalculateRay(mouseLocation, camera.GetViewMatrix(),
                camera.GetProjectMatrix(), graphicsDevice.Viewport);

            var position = ray.Position - ray.Direction * (ray.Position.Y / ray.Direction.Y);
            var chunkPosition = ChunkLoaderEntity.CalculateChunkPosition(position);

            //The closest chunk and all its surrounding chunks.
            var chunkPositions = new[]
            {
                chunkPosition,
                (chunkPosition.Item1 - 1, chunkPosition.Item2),
                (chunkPosition.Item1, chunkPosition.Item2 - 1),
                (chunkPosition.Item1 + 1, chunkPosition.Item2),
                (chunkPosition.Item1, chunkPosition.Item2 + 1),
                (chunkPosition.Item1 - 1, chunkPosition.Item2 - 1),
                (chunkPosition.Item1 - 1, chunkPosition.Item2 + 1),
                (chunkPosition.Item1 + 1, chunkPosition.Item2 + 1),
                (chunkPosition.Item1 + 1, chunkPosition.Item2 - 1)
            };

            foreach (var pos in chunkPositions)
            {
                var chunk = chunkLoaderEntity.GetChunk(pos);
                
                if (chunk != null)
                {
                    for (var x = 0; x < chunk.GetTileBoundingBoxes().GetLength(0); x++) {
                        for (var y = 0; y < chunk.GetTileBoundingBoxes().GetLength(1); y++) {
                            var intersects = ray.Intersects(chunk.GetTileBoundingBoxes()[x, y]);
                                                
                            if (intersects != null)
                            {
                                return ray.Position + ray.Direction * (float) intersects;        
                            }
                        }
                    }
                }
            }

            return null;
        }
        
        private static Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport) {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                projection,
                view,
                Matrix.Identity);
 
            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 10f),
                projection,
                view,
                Matrix.Identity);

            Vector3 direction = nearPoint - farPoint;
            
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        #endregion

        #region Getters and Setters

        public Vector3 GetGridPosition()
        {
            return _gridPosition;
        }

        public TerrainEntity.TileCorner GetSelectedCorner()
        {
            return _selectedCorner;
        }

        #endregion
    }
}