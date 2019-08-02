using System;
using System.Collections.Generic;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using DProject.Manager;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Debug
{
    public class DebugEntity : AbstractAwareEntity, IInitialize, IUpdateable, IDrawable
    {
        private readonly List<CameraEntity> _cameraEntities;
        private readonly ChunkLoaderEntity _chunkLoaderEntity;
        private readonly PointerEntity _pointerEntity;

        private readonly AxisEntity _axisEntity;
        private readonly AxisEntity _pointerAxisEntity;
        
        private readonly List<LineFrameEntity> _lineFrameEntities;

        private GraphicsDevice _graphicsDevice;
                
        private readonly RasterizerState _rasterizerStateWireFrame;
        private readonly RasterizerState _rasterizerStateSolid;
        
        public DebugEntity(EntityManager entityManager) : base(entityManager,Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _pointerEntity = entityManager.GetPointerEntity();
            
            _axisEntity = new AxisEntity(Vector3.Zero);
            _pointerAxisEntity = new AxisEntity(Vector3.Zero);
            
            var cameraEntities = EntityManager.GetCameraEntities();
            
            _cameraEntities = cameraEntities;
            _chunkLoaderEntity = EntityManager.GetChunkLoaderEntity();
            
            _lineFrameEntities = new List<LineFrameEntity>();
            
            _rasterizerStateWireFrame = new RasterizerState();
            _rasterizerStateSolid = new RasterizerState();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            _rasterizerStateSolid.CullMode = CullMode.CullCounterClockwiseFace;
            _rasterizerStateSolid.FillMode = FillMode.Solid;

            _rasterizerStateWireFrame.CullMode = CullMode.CullCounterClockwiseFace;
            _rasterizerStateWireFrame.FillMode = FillMode.WireFrame;
            
            _axisEntity.Initialize(graphicsDevice);
            _pointerAxisEntity.Initialize(graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                _graphicsDevice.RasterizerState = _rasterizerStateWireFrame;
            
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                UpdateChunkFrames();

            _axisEntity.SetPosition(_pointerEntity.GetPosition());
            _pointerAxisEntity.SetPosition(_pointerEntity.GetGridPosition());
        }

        public void UpdateChunkFrames()
        {
            _lineFrameEntities.Clear();

            var chunkIterator = _chunkLoaderEntity.GetLoadedChunks();
            
            while (chunkIterator.MoveNext())
            {
                var chunk = chunkIterator.Current.Value;
                
                if (chunk != null)
                {
                    Color color;
                
                    switch (chunk.GetChunkData().ChunkStatus)
                    {
                        case ChunkStatus.Unserialized:
                            color = Color.Blue;
                            break;
                        case ChunkStatus.Changed:
                            color = Color.Red;
                            break;
                        case ChunkStatus.Current:
                            color = Color.Green;
                            break;
                        default:
                            color = Color.White;
                            break;
                    }
                
                    var entity = new LineFrameEntity(chunk.GetPosition(), ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, color);
                    entity.Initialize(_graphicsDevice);
                
                    _lineFrameEntities.Add(entity);
                }
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            _axisEntity.Draw(activeCamera);
            _pointerAxisEntity.Draw(activeCamera);

            foreach (var lineFrame in _lineFrameEntities)
                lineFrame.Draw(activeCamera);
        }
    }
}