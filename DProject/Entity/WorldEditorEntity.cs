using System;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Debug;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class WorldEditorEntity : AbstractAwareEntity, IInitialize, IUpdateable, IDrawable
    {
        private readonly CornerIndicatorEntity _cornerIndicatorEntity;

        private readonly PointerEntity _pointerEntity;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;

        public enum Tools { Select, Flatten, Raise, Paint, ObjectPlacer }
        private Tools _tools;

        private ushort _flattenHeight;
        private int _brushSize;

        private ushort _selectedObject;
        private Rotation _selectedRotation;
        private ushort _selectedTexture;
        private ushort _selectedColor;
        
        public WorldEditorEntity(EntityManager entityManager) : base(entityManager, Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _cornerIndicatorEntity = new CornerIndicatorEntity(Vector3.Zero, TerrainEntity.TileCorner.BottomRight, Microsoft.Xna.Framework.Color.Cyan);

            _pointerEntity = EntityManager.GetPointerEntity();
            _chunkLoaderEntity = EntityManager.GetChunkLoaderEntity();
            
            _selectedObject = Props.GetDefaultPropId();
            _selectedRotation = Type.Enum.Rotation.North;
            _selectedTexture = Textures.GetDefaultTextureId();
            _selectedColor = Colors.GetDefaultColorId();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _cornerIndicatorEntity.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            if (!_chunkLoaderEntity.IsLoadingChunks())
                UseTool();
        }

        public void Draw(CameraEntity activeCamera)
        {
            if(_brushSize < 1)
                _cornerIndicatorEntity.Draw(activeCamera);
        }

        #region Tools

        private void UseTool()
        {
            var precisePosition = _pointerEntity.GetPosition();
            var position = _pointerEntity.GetGridPosition();
                
            if (_chunkLoaderEntity.GetChunk(position) != null)
            {
                switch (_tools)
                {
                    case Tools.Select:
                        Color(position, precisePosition);
                        break;
                    case Tools.Flatten:
                        Flatten(precisePosition, position);
                        break;
                    case Tools.Raise:
                        Raise(position, precisePosition);
                        break;
                    case Tools.Paint:
                        Texture(position, precisePosition);
                        break;
                    case Tools.ObjectPlacer:
                        PlaceObject(position);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ChangeHeight(Vector3 position, Vector3 precisePosition, ushort height) { }
        
        private void ChangeColor(Vector3 position, Vector3 precisePosition, ushort color) { }

        private void Color(Vector3 position, Vector3 precisePosition) { }

        private void Raise(Vector3 position, Vector3 precisePosition) { }

        private void Flatten(Vector3 precisePosition, Vector3 position) { }
        
        private void PlaceObject(Vector3 position) { }

        private void Texture(Vector3 position, Vector3 precisePosition) { }
        
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

        public void SetSelectedRotation(Rotation rotation)
        {
            _selectedRotation = rotation;
        }

        public ushort GetFlattenHeight()
        {
            return _flattenHeight;
        }
        
        public void SetFlattenHeight(ushort height)
        {
            _flattenHeight = height;
        }

        public int GetBrushSize()
        {
            return _brushSize;
        }

        public void SetBrushSize(int size)
        {            
            _brushSize = size;
        }

        public ushort GetActiveTexture()
        {
            return _selectedTexture;
        }

        public void SetActiveTexture(ushort textureId)
        {
            _selectedTexture = textureId;
        }

        #endregion
    }
}