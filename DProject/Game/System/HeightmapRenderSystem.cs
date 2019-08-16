using DProject.Game.Component;
using DProject.List;
using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class HeightmapRenderSystem : EntityDrawSystem
    {
        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<LoadedHeightmapComponent> _heightmapMapper;
        private ComponentMapper<TransformComponent> _transformMapper;

        public HeightmapRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.All(typeof(LoadedHeightmapComponent), typeof(TransformComponent)))
        {
            _shaderManager = shaderManager;
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _heightmapMapper = mapperService.GetMapper<LoadedHeightmapComponent>();
            _transformMapper = mapperService.GetMapper<TransformComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var loadedHeightmapComponent = _heightmapMapper.Get(entity);
                var transformComponent = _transformMapper.Get(entity);
                
                Draw(transformComponent, loadedHeightmapComponent, Textures.AtlasList["floor_textures"].AtlasTexture2D,  _shaderManager);
            }
        }
        
        public void Draw(TransformComponent transformComponent, LoadedHeightmapComponent loadedHeightmapComponent, Texture2D texture2D, ShaderManager shaderManager)
        {
            AbstractEffect effect = null;
            
            switch (shaderManager.CurrentRenderTarget)
            {
                case ShaderManager.RenderTarget.Depth:
                    effect = shaderManager.DepthEffect;
                    effect.World = transformComponent.WorldMatrix;
                    break;
                case ShaderManager.RenderTarget.Refraction:
                case ShaderManager.RenderTarget.Reflection:
                case ShaderManager.RenderTarget.Final:
                    effect = shaderManager.TerrainEffect;
                    effect.World = transformComponent.WorldMatrix;
                    break;
                default:
                    return;
            }
            
            _graphicsDevice.SetVertexBuffer(loadedHeightmapComponent.VertexBuffer);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (loadedHeightmapComponent.PrimitiveCount != 0)
                    _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, loadedHeightmapComponent.PrimitiveCount);
            }
        }
    }
}
