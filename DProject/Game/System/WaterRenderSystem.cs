using DProject.Game.Component;
using DProject.Manager.System.Terrain;
using DProject.Type.Rendering;
using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class WaterRenderSystem : EntityDrawSystem
    {
        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<WaterPlaneComponent> _waterPlaneMapper;

        private readonly VertexBuffer _vertexBuffer;

        public WaterRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.All(typeof(TransformComponent), typeof(WaterPlaneComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _shaderManager = shaderManager;
            
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTextureColorNormal), 6, BufferUsage.WriteOnly);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            _waterPlaneMapper = mapperService.GetMapper<WaterPlaneComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var waterPlane = _waterPlaneMapper.Get(entity);

                AbstractEffect effect;

                switch (_shaderManager.CurrentRenderTarget)
                {
                    case ShaderManager.RenderTarget.Final:
                        effect = _shaderManager.WaterEffect;
                        effect.World = transform.WorldMatrix;
                        break;
                    default:
                        return;
                }

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
