using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class PrimitiveRenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<PrimitiveComponent> _primitiveMapper;
        private ComponentMapper<TransformComponent> _transformMapper;

        public PrimitiveRenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(PrimitiveComponent), typeof(TransformComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _primitiveMapper = mapperService.GetMapper<PrimitiveComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var primitive = _primitiveMapper.Get(entity);
                var transform = _transformMapper.Get(entity);
                
                ShaderManager.Instance.GBufferEffect.World = transform.WorldMatrix;
                ShaderManager.Instance.DrawPrimitive(ShaderManager.Instance.GBufferEffect, primitive.Type);
            }
        }
    }
}
