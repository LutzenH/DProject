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
    public class WorldEditorEntity : AbstractEntity, IInitialize,IUpdateable, IDrawable
    {
        private readonly AxisEntity _axisEntity;
        private readonly AxisEntity _pointerEntity;
        
        private readonly EntityManager _entityManager;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;
        private GraphicsDevice _graphicsDevice;
        
        public enum Tools { Select, Flatten, Raise, Paint, ObjectPlacer }
        private Tools _tools;

        private float _flattenHeight;
        private int _brushSize;
        
        public WorldEditorEntity(EntityManager entityManager, ChunkLoaderEntity chunkLoaderEntity) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _axisEntity = new AxisEntity(Vector3.Zero);
            _pointerEntity = new AxisEntity(Vector3.Zero);
            _entityManager = entityManager;
            _chunkLoaderEntity = chunkLoaderEntity;
        }

        public override void LoadContent(ContentManager content) { }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _axisEntity.Initialize(graphicsDevice);
            _pointerEntity.Initialize(graphicsDevice);

            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.D1) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D1))
                _tools = Tools.Select;
            if (Keyboard.GetState().IsKeyUp(Keys.D2) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D2))
                _tools = Tools.Flatten;
            if (Keyboard.GetState().IsKeyUp(Keys.D3) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D3))
                _tools = Tools.Raise;
            if (Keyboard.GetState().IsKeyUp(Keys.D4) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D4))
                _tools = Tools.Paint;
            if (Keyboard.GetState().IsKeyUp(Keys.D5) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D5))
                _tools = Tools.ObjectPlacer;

            SetBrushSize();

            if (!_chunkLoaderEntity.IsLoadingChunks())
            {
                var mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                var position = CalculatePosition(mouseLocation);
                
                switch (_tools)
                {
                    case Tools.Select:
                        
                        break;
                    case Tools.Flatten:
                        Flatten(position);
                        break;
                    case Tools.Raise:
                        Raise(position);
                        break;
                    case Tools.Paint:
                        Paint(position);
                        break;
                    case Tools.ObjectPlacer:
                        PlaceObject(position, mouseLocation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _axisEntity.SetPosition(new Vector3(position.X, 0, position.Z));
                _pointerEntity.SetPosition(position);
            }
        }

        private void SetBrushSize()
        {
            _brushSize = Math.Abs(Mouse.GetState().ScrollWheelValue / 120);
            _brushSize %= 8;
        }

        private void Raise(Vector3 position)
        {
            Vector2 localChunkPosition = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));

            int x = (int)localChunkPosition.X;
            int y = (int) localChunkPosition.Y;

            float vertexHeight = _chunkLoaderEntity.GetChunk(position).GetVertexHeight(x, y, TerrainEntity.Corner.TopLeft);
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _chunkLoaderEntity.GetChunk(position).ChangeTileHeight(vertexHeight + 0.01f, x, y);
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                _chunkLoaderEntity.GetChunk(position).ChangeTileHeight(vertexHeight - 0.01f, x, y);
            }
        }

        private void PlaceObject(Vector3 position, Vector2 mouseLocation)
        {        
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

        private void Flatten(Vector3 position)
        {
            Vector2 localChunkPosition = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            int x = (int)localChunkPosition.X;
            int y = (int) localChunkPosition.Y;
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                _chunkLoaderEntity.GetChunk(position).ChangeTileHeight(_flattenHeight, x, y);

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                _flattenHeight = _chunkLoaderEntity.GetChunk(position).GetTileHeight(x, y);
        }

        private void Paint(Vector3 position)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector2 localChunkPosition = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));

                int x = (int)localChunkPosition.X;
                int y = (int) localChunkPosition.Y;

                _chunkLoaderEntity.GetChunk(position).ChangeTexture("metal", x, y);
            }
        }

        private Vector3 CalculatePosition(Vector2 mouseLocation)
        {            
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
            
            return position;
        }
        
        public void Draw(CameraEntity activeCamera)
        {
            _axisEntity.Draw(activeCamera);
            _pointerEntity.Draw(activeCamera);
        }
        
        public Tools GetCurrentTool()
        {
            return _tools;
        }

        public float GetFlattenHeight()
        {
            return _flattenHeight;
        }

        public int GetBrushSize()
        {
            return _brushSize;
        }
    }
}