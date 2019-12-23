using DProject.Game.Component;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class WaterRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<WaterPlaneComponent> _waterPlaneMapper;

        private readonly VertexBuffer _vertexBuffer;

        public WaterRenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(TransformComponent), typeof(WaterPlaneComponent)))
        {
            _graphicsDevice = graphicsDevice;
            
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTextureColorNormal), 6, BufferUsage.WriteOnly);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            _waterPlaneMapper = mapperService.GetMapper<WaterPlaneComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var waterPlane = _waterPlaneMapper.Get(entity);

                var effect = ShaderManager.Instance.WaterEffect;
                effect.World = transform.WorldMatrix;

                //TODO: This only needs to happen once and not every draw-call.
                _vertexBuffer.SetData(waterPlane.VertexList);
                
                _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                }
            }
        }
    }
}
