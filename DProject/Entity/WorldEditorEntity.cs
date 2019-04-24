using System;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager;
using DProject.Type.Enum;
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
        private readonly CornerIndicatorEntity _cornerIndicatorEntity;
        
        private readonly EntityManager _entityManager;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;
        private GraphicsDevice _graphicsDevice;
        
        public enum Tools { Select, Flatten, Raise, Paint, ObjectPlacer }
        private Tools _tools;

        private byte _flattenHeight;
        private int _brushSize;

        private ushort _activeTexture;
        
        private ushort _selectedObject;
        private Rotation _selectedRotation;
        private ushort _selectedColor;
                
        private byte _currentFloor;
        
        public WorldEditorEntity(EntityManager entityManager, ChunkLoaderEntity chunkLoaderEntity) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _axisEntity = new AxisEntity(Vector3.Zero);
            _pointerEntity = new AxisEntity(Vector3.Zero);
            _cornerIndicatorEntity = new CornerIndicatorEntity(Vector3.Zero, TerrainEntity.TileCorner.BottomRight, Color.Cyan);
            
            _entityManager = entityManager;
            _chunkLoaderEntity = chunkLoaderEntity;

            _activeTexture = Textures.GetDefaultTextureId();
            _selectedObject = Props.GetDefaultPropId();
            _selectedRotation = Type.Enum.Rotation.North;
        }

        public override void LoadContent(ContentManager content) { }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _axisEntity.Initialize(graphicsDevice);
            _pointerEntity.Initialize(graphicsDevice);
            _cornerIndicatorEntity.Initialize(graphicsDevice);

            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.OemPlus) && Game1.PreviousKeyboardState.IsKeyDown(Keys.OemPlus))
            {
                _currentFloor++;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.OemMinus) && Game1.PreviousKeyboardState.IsKeyDown(Keys.OemMinus))
            {
                if (_currentFloor != 0)
                    _currentFloor--;
            }

            if (!_chunkLoaderEntity.IsLoadingChunks())
                UseTool();

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.LeftControl) &&
                Game1.PreviousKeyboardState.IsKeyDown(Keys.LeftControl) && Game1.PreviousKeyboardState.IsKeyUp(Keys.S))
            {
                _chunkLoaderEntity.SerializeChangedChunks();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftControl) &&
                Game1.PreviousKeyboardState.IsKeyDown(Keys.LeftControl) && Game1.PreviousKeyboardState.IsKeyUp(Keys.R))
            {
                _chunkLoaderEntity.ReloadChangedChunks();
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            _axisEntity.Draw(activeCamera);
            _pointerEntity.Draw(activeCamera);
            
            if(_brushSize < 1)
                _cornerIndicatorEntity.Draw(activeCamera);
        }

        #region Tools
        
        private void UseTool()
        {
            var mouseLocation = Game1.GetAdjustedMousePosition();
            var position = CalculatePosition(mouseLocation);
            var precisePosition = CalculatePrecisePosition(mouseLocation);
                
            if (_chunkLoaderEntity.GetChunk(position) != null)
            {
                switch (_tools)
                {
                    case Tools.Select:
                        ColorPainter(position, precisePosition);
                        break;
                    case Tools.Flatten:
                        Flatten(precisePosition, position);
                        break;
                    case Tools.Raise:
                        Raise(position, precisePosition);
                        break;
                    case Tools.Paint:
                        Paint(position, precisePosition);
                        break;
                    case Tools.ObjectPlacer:
                        PlaceObject(position);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ChangeHeight(Vector3 position, Vector3 precisePosition, byte height)
        {
            if (_brushSize > 0)
            {
                _chunkLoaderEntity.ChangeTileHeight(height, position, _currentFloor, _brushSize); 
            }
            else
            {
                var tileCorner = CalculateCorner(precisePosition);
                
                _chunkLoaderEntity.ChangeCornerHeight(height, position, _currentFloor, tileCorner);
            }
        }
        
        private void ChangeColor(Vector3 position, Vector3 precisePosition, ushort color)
        {
            if (_brushSize > 0)
            {
                _chunkLoaderEntity.ChangeTileColor(color, position, _currentFloor, _brushSize); 
            }
            else
            {
                var tileCorner = CalculateCorner(precisePosition);
                
                _chunkLoaderEntity.ChangeCornerColor(color, position, _currentFloor, tileCorner);
            }
        }

        private void ColorPainter(Vector3 position, Vector3 precisePosition)
        {
            var tileCorner = CalculateCorner(precisePosition);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ChangeColor(position, precisePosition, _selectedColor);
            }

            _axisEntity.SetPosition(precisePosition);
            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }

        private void Raise(Vector3 position, Vector3 precisePosition)
        {
            var tileCorner = CalculateCorner(precisePosition);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                byte? height = _chunkLoaderEntity.GetVertexHeight(new Vector2(position.X, position.Z), tileCorner, _currentFloor);
                
                if(height != 255)
                    height++;
                
                ChangeHeight(position, precisePosition, (byte)height);
            } 
            else if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                byte? height = _chunkLoaderEntity.GetVertexHeight(new Vector2(position.X, position.Z), tileCorner, _currentFloor);
                
                if(height != 0)
                    height--;
                
                ChangeHeight(position, precisePosition, (byte)height);
            }

            _axisEntity.SetPosition(precisePosition);
            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }

        private void Flatten(Vector3 precisePosition, Vector3 position)
        {        
            var (xFloat, yFloat) = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            var x = (int) xFloat;
            var y = (int) yFloat;
            
            var tileCorner = CalculateCorner(precisePosition);
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                ChangeHeight(position, precisePosition, _flattenHeight);
            else if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (_brushSize > 0)
                {
                    _flattenHeight = _chunkLoaderEntity.GetChunk(position).GetTileHeight(x, y, _currentFloor);
                }
                else
                {
                    _flattenHeight = _chunkLoaderEntity.GetChunk(position).GetVertexHeight(x, y, _currentFloor, tileCorner);
                }
            }
            
            _axisEntity.SetPosition(precisePosition);
            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }
        
        private void PlaceObject(Vector3 position)
        {            
            _axisEntity.SetPosition(position);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                _chunkLoaderEntity.PlaceProp(position, _currentFloor, _selectedRotation, _selectedObject);
                return;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && Game1.PreviousMouseState.RightButton == ButtonState.Released)
            {
                _chunkLoaderEntity.RemoveProp(position, _currentFloor);
                return;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed && Game1.PreviousMouseState.MiddleButton == ButtonState.Released)
            {
                switch (_selectedRotation)
                {
                    case Type.Enum.Rotation.North:
                        _selectedRotation = Type.Enum.Rotation.East;
                        break;
                    case Type.Enum.Rotation.East:
                        _selectedRotation = Type.Enum.Rotation.South;
                        break;
                    case Type.Enum.Rotation.South:
                        _selectedRotation = Type.Enum.Rotation.West;
                        break;
                    case Type.Enum.Rotation.West:
                        _selectedRotation = Type.Enum.Rotation.North;
                        break;
                }
            }
        }

        private void Paint(Vector3 position, Vector3 precisePosition)
        {
            _axisEntity.SetPosition(precisePosition);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !UIManager.ClickedUI)
            {                
                var corner = CalculateCorner(precisePosition);
                var alternativeTriangle = (corner == TerrainEntity.TileCorner.BottomLeft || corner == TerrainEntity.TileCorner.BottomRight);
                
                _chunkLoaderEntity.ChangeTileTexture(_activeTexture, position, _currentFloor, _brushSize, alternativeTriangle);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !UIManager.ClickedUI)
            {
                var corner = CalculateCorner(precisePosition);
                var alternativeTriangle = (corner == TerrainEntity.TileCorner.BottomLeft || corner == TerrainEntity.TileCorner.BottomRight);
                
                _chunkLoaderEntity.ChangeTileTexture(null, position, _currentFloor, _brushSize, alternativeTriangle);
            }
        }
        
        #endregion

        #region Calculations
        
        private TerrainEntity.TileCorner CalculateCorner(Vector3 precisePosition)
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

        private Vector3 CalculatePosition(Vector2 mouseLocation)
        {
            var position = CalculatePrecisePosition(mouseLocation);

            return new Vector3((int)Math.Round(position.X), position.Y, (int)Math.Round(position.Z));
        }

        private Vector3 CalculatePrecisePosition(Vector2 mouseLocation)
        {
            Ray ray = Game1.CalculateRay(mouseLocation, _entityManager.GetActiveCamera().GetViewMatrix(),
                _entityManager.GetActiveCamera().GetProjectMatrix(), _graphicsDevice.Viewport);
            
            Vector3 position = Vector3.Zero;
            
            if (ray.Direction.Y != 0)
            {
                Vector3 tempPosition = ray.Position - ray.Direction * (ray.Position.Y / ray.Direction.Y);        
                position = new Vector3(tempPosition.X, 0, tempPosition.Z);
            }

            var chunk = _chunkLoaderEntity.GetChunk(position);

            if (chunk != null)
            {
                for (var x = 0; x < chunk.GetTileBoundingBoxes(_currentFloor).GetLength(0); x++) {
                    for (var y = 0; y < chunk.GetTileBoundingBoxes(_currentFloor).GetLength(1); y++) {
                        var intersects = ray.Intersects(_chunkLoaderEntity.GetChunk(position).GetTileBoundingBoxes(_currentFloor)[x, y]);
                                                
                        if (intersects != null)
                        {
                            position = ray.Position + ray.Direction * (float) intersects;        
                        }
                    }
                }
            }
            
            return position;
        }

        #endregion
        
        #region Getters and Setters
        
        public Tools GetCurrentTool()
        {
            return _tools;
        }
        
        public void SetCurrentTool(Tools tool)
        {
            _tools = tool;
        }

        public int GetCurrentFloor()
        {
            return _currentFloor;
        }

        public ushort GetSelectedColor()
        {
            return _selectedColor;
        }

        public void SetSelectedColor(ushort colorId)
        {
            _selectedColor = colorId;
        }

        public ushort GetSelectedObject()
        {
            return _selectedObject;
        }

        public void SetSelectedObject(ushort objectId)
        {
            _selectedObject = objectId;
        }
        
        public Rotation GetSelectedRotation()
        {
            return _selectedRotation;
        }

        public float GetFlattenHeight()
        {
            return _flattenHeight;
        }

        public int GetBrushSize()
        {
            return _brushSize;
        }

        public void SetBrushSize(int size)
        {            
            _brushSize = size;
        }

        public void SetActiveTexture(ushort textureId)
        {
            _activeTexture = textureId;
        }

        #endregion
    }
}