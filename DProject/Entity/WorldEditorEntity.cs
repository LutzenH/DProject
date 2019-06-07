using System;
using System.Collections.Generic;
using System.Drawing;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager;
using DProject.Type.Enum;
using DProject.Type.Rendering;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;
using Object = DProject.Type.Serializable.Chunk.Object;

namespace DProject.Entity
{
    public class WorldEditorEntity : AbstractAwareEntity, IInitialize, IUpdateable, IDrawable
    {
        private readonly CornerIndicatorEntity _cornerIndicatorEntity;

        private readonly PointerEntity _pointerEntity;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;

        public enum Tools { Select, Flatten, Raise, Paint, ObjectPlacer }
        private Tools _tools;

        private byte _flattenHeight;
        private int _brushSize;

        private ushort _activeTexture;
        
        private ushort _selectedObject;
        private Rotation _selectedRotation;
        private ushort _selectedColor;
        
        public WorldEditorEntity(EntityManager entityManager) : base(entityManager, Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _cornerIndicatorEntity = new CornerIndicatorEntity(Vector3.Zero, TerrainEntity.TileCorner.BottomRight, Color.Cyan);

            _pointerEntity = EntityManager.GetPointerEntity();
            _chunkLoaderEntity = EntityManager.GetChunkLoaderEntity();

            _activeTexture = Textures.GetDefaultTextureId();
            _selectedObject = Props.GetDefaultPropId();
            _selectedRotation = Type.Enum.Rotation.North;
        }

        public override void LoadContent(ContentManager content) { }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _cornerIndicatorEntity.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
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
            if(_brushSize < 1)
                _cornerIndicatorEntity.Draw(activeCamera);
        }

        #region Tools

        public static List<ChunkData> GenerateChunkDataUsingHeightmap(Bitmap image, int xPos, int yPos)
        {
            var list = new List<ChunkData>();

            if (image.Width % ChunkLoaderEntity.ChunkSize == 1 
                && image.Height % ChunkLoaderEntity.ChunkSize == 1)
            {
                for (var chunkX = 0; chunkX < (image.Width - 1) / ChunkLoaderEntity.ChunkSize; chunkX++)
                {
                    for (var chunkY = 0; chunkY < (image.Height - 1) / ChunkLoaderEntity.ChunkSize; chunkY++)
                    {
                        var heightmap = new byte[ChunkLoaderEntity.ChunkSize + 1, ChunkLoaderEntity.ChunkSize + 1];

                        for (var xPix = 0; xPix < ChunkLoaderEntity.ChunkSize + 1; xPix++)
                        {
                            for (var yPix = 0; yPix < ChunkLoaderEntity.ChunkSize + 1; yPix++)
                            {
                                var xPixel = xPix + ChunkLoaderEntity.ChunkSize * chunkX;
                                var yPixel = yPix + ChunkLoaderEntity.ChunkSize * chunkY;
                                
                                heightmap[xPix, yPix] = image.GetPixel(xPixel, yPixel).R;
                            }
                        }
                        
                        Tile[][,] tiles = new Tile[1][,];
                
                        tiles[0] = HeightMap.GenerateTileMap(heightmap);
                
                        var chunkdata = new ChunkData()
                        {
                            ChunkPositionX = xPos + chunkX,
                            ChunkPositionY = yPos + chunkY,
                            Tiles = tiles,
                    
                            Objects = Object.GenerateObjects(0, 0, 1, 0),
                    
                            ChunkStatus = ChunkStatus.Unserialized
                        };
                        
                        list.Add(chunkdata);
                    }
                }

                return list;
            }

            return null;
        }

        public static void SerializeChunkDataList(List<ChunkData> chunkData)
        {
            foreach (var chunk in chunkData) 
                TerrainEntity.Serialize(chunk);
        }

        private void UseTool()
        {
            var precisePosition = _pointerEntity.GetPosition();
            var position = _pointerEntity.GetGridPosition();
                
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
                _chunkLoaderEntity.ChangeTileHeight(height, position, _pointerEntity.GetCurrentFloor(), _brushSize); 
            }
            else
            {
                var tileCorner = _pointerEntity.GetSelectedCorner();
                
                _chunkLoaderEntity.ChangeCornerHeight(height, position, _pointerEntity.GetCurrentFloor(), tileCorner);
            }
        }
        
        private void ChangeColor(Vector3 position, Vector3 precisePosition, ushort color)
        {
            if (_brushSize > 0)
            {
                _chunkLoaderEntity.ChangeTileColor(color, position, _pointerEntity.GetCurrentFloor(), _brushSize); 
            }
            else
            {
                var tileCorner = _pointerEntity.GetSelectedCorner();
                
                _chunkLoaderEntity.ChangeCornerColor(color, position, _pointerEntity.GetCurrentFloor(), tileCorner);
            }
        }

        private void ColorPainter(Vector3 position, Vector3 precisePosition)
        {
            var tileCorner = _pointerEntity.GetSelectedCorner();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ChangeColor(position, precisePosition, _selectedColor);
            }

            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }

        private void Raise(Vector3 position, Vector3 precisePosition)
        {
            var tileCorner = _pointerEntity.GetSelectedCorner();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                byte? height = _chunkLoaderEntity.GetVertexHeight(new Vector2(position.X, position.Z), tileCorner, _pointerEntity.GetCurrentFloor());
                
                if(height != 255)
                    height++;
                
                ChangeHeight(position, precisePosition, (byte)height);
            } 
            else if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                byte? height = _chunkLoaderEntity.GetVertexHeight(new Vector2(position.X, position.Z), tileCorner, _pointerEntity.GetCurrentFloor());
                
                if(height != 0)
                    height--;
                
                ChangeHeight(position, precisePosition, (byte)height);
            }

            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }

        private void Flatten(Vector3 precisePosition, Vector3 position)
        {        
            var (xFloat, yFloat) = ChunkLoaderEntity.GetLocalChunkPosition(new Vector2(position.X, position.Z));
            
            var x = (int) xFloat;
            var y = (int) yFloat;

            var tileCorner = _pointerEntity.GetSelectedCorner();
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                ChangeHeight(position, precisePosition, _flattenHeight);
            else if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (_brushSize > 0)
                {
                    _flattenHeight = _chunkLoaderEntity.GetChunk(position).GetTileHeight(x, y, _pointerEntity.GetCurrentFloor());
                }
                else
                {
                    _flattenHeight = _chunkLoaderEntity.GetChunk(position).GetVertexHeight(x, y, _pointerEntity.GetCurrentFloor(), tileCorner);
                }
            }
            
            _cornerIndicatorEntity.SetPosition(position);
            _cornerIndicatorEntity.SetRotation(tileCorner);
        }
        
        private void PlaceObject(Vector3 position)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                _chunkLoaderEntity.PlaceProp(position, _pointerEntity.GetCurrentFloor(), _selectedRotation, _selectedObject);
                return;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && Game1.PreviousMouseState.RightButton == ButtonState.Released)
            {
                _chunkLoaderEntity.RemoveProp(position, _pointerEntity.GetCurrentFloor());
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
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !UIManager.ClickedUI)
            {
                var corner = _pointerEntity.GetSelectedCorner();
                var alternativeTriangle = (corner == TerrainEntity.TileCorner.BottomLeft || corner == TerrainEntity.TileCorner.BottomRight);
                
                _chunkLoaderEntity.ChangeTileTexture(_activeTexture, position, _pointerEntity.GetCurrentFloor(), _brushSize, alternativeTriangle);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !UIManager.ClickedUI)
            {
                var corner = _pointerEntity.GetSelectedCorner();
                var alternativeTriangle = (corner == TerrainEntity.TileCorner.BottomLeft || corner == TerrainEntity.TileCorner.BottomRight);
                
                _chunkLoaderEntity.ChangeTileTexture(null, position, _pointerEntity.GetCurrentFloor(), _brushSize, alternativeTriangle);
            }
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