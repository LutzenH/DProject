using System;
using System.Collections.Generic;
using DProject.Entity.Camera;
using DProject.Entity.Chunk;
using DProject.Entity.Interface;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Debug
{
    public class DebugEntity : AbstractEntity, IInitialize, IUpdateable, IDrawable
    {
        private PropEntity[] cameraProps;
        
        private List<CameraEntity> _cameraEntities;
        private ChunkLoaderEntity _chunkLoaderEntity;

        private List<LineFrameEntity> _lineFrameEntities;

        private GraphicsDevice _graphicsDevice;
                
        public DebugEntity(List<CameraEntity> cameraEntities, ChunkLoaderEntity chunkLoaderEntity) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            cameraProps = new PropEntity[cameraEntities.Count];
            
            _cameraEntities = cameraEntities;
            _chunkLoaderEntity = chunkLoaderEntity;
            
            _lineFrameEntities = new List<LineFrameEntity>();
            
            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i] = new PropEntity(_cameraEntities[i].GetPosition(), Quaternion.Identity, cameraEntities[i].GetScale(), 5); //camera
            }
        }

        public override void LoadContent(ContentManager content)
        {
            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i].LoadContent(content);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                UpdateChunkFrames();
            
            for (int i = 0; i < cameraProps.Length; i++)
            {
                Vector3 cameraDirection = _cameraEntities[i].GetCameraDirection();

                float yaw = (float) Math.Asin(cameraDirection.X / Math.Cos(Math.Asin(cameraDirection.Y)));
                float pitch = (float) Math.Asin(cameraDirection.Y);
          
                cameraProps[i].SetPosition(_cameraEntities[i].GetPosition());
                cameraProps[i].SetRotation(-pitch, yaw+ 1.5f, 0f);
            }
        }

        public void UpdateChunkFrames()
        {
            _lineFrameEntities.Clear();

            foreach (var chunk in _chunkLoaderEntity.GetLoadedChunks())
            {
                if (chunk != null)
                {
                    Color color;
                
                    switch (chunk.ChunkStatus)
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
                
                    LineFrameEntity entity = new LineFrameEntity(chunk.GetPosition(), ChunkLoaderEntity.ChunkSize, ChunkLoaderEntity.ChunkSize, color);
                    entity.Initialize(_graphicsDevice);
                
                    _lineFrameEntities.Add(entity);
                }
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            for (int i = 0; i < cameraProps.Length; i++)
            {
                cameraProps[i].Draw(activeCamera);
            }

            foreach (var lineFrame in _lineFrameEntities)
                lineFrame.Draw(activeCamera);
        }
    }
}