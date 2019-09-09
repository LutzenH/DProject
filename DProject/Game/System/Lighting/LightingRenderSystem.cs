using DProject.Game.Component.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager.System.Lighting
{
    public class LightingRenderSystem : EntityDrawSystem
    {
        private GraphicsDevice _graphicsDevice;
        private ShaderManager _shaderManager;

        private ComponentMapper<DirectionalLightComponent> _directionalLightMapper;
        private ComponentMapper<PointLightComponent> _pointLightMapper;
        
        public LightingRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.One(typeof(DirectionalLightComponent), typeof(PointLightComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _shaderManager = shaderManager;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _directionalLightMapper = mapperService.GetMapper<DirectionalLightComponent>();
            _pointLightMapper = mapperService.GetMapper<PointLightComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            var previousCullMode = _graphicsDevice.RasterizerState.CullMode;
            
            _graphicsDevice.SetRenderTarget(_shaderManager.Lights);
            _graphicsDevice.Clear(Color.Transparent);
            
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.BlendState.AlphaBlendFunction = BlendFunction.Max;
            _graphicsDevice.BlendState.AlphaSourceBlend = Blend.One;
            _graphicsDevice.BlendState.AlphaDestinationBlend = Blend.One;
            
            foreach (var entity in ActiveEntities)
            {
                if (_directionalLightMapper.Has(entity))
                {
                    var light = _directionalLightMapper.Get(entity);

                    _shaderManager.DirectionalLightEffect.LightDirection = light.Direction;
                    _shaderManager.DirectionalLightEffect.LightColor = light.Color;
                
                    _shaderManager.DrawFullscreenQuad(_shaderManager.DirectionalLightEffect);
                }

                if (_pointLightMapper.Has(entity))
                {
                    var light = _pointLightMapper.Get(entity);

                    _shaderManager.PointLightEffect.LightColor = light.Color;
                    _shaderManager.PointLightEffect.LightIntensity = light.Intensity;
                    _shaderManager.PointLightEffect.LightPosition = light.Position;
                    _shaderManager.PointLightEffect.LightRadius = light.Radius;
                    _shaderManager.PointLightEffect.World = light.WorldMatrix;

                    var cameraToCenter = Vector3.Distance(CameraSystem.ActiveLens.Position, light.Position);
                    _graphicsDevice.RasterizerState.CullMode = cameraToCenter <= light.Radius ? CullMode.CullClockwiseFace : CullMode.CullCounterClockwiseFace;

                    _graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
                    _shaderManager.DrawPrimitive(_shaderManager.PointLightEffect, PrimitiveType.Sphere);
                }
            }

            _graphicsDevice.RasterizerState.CullMode = previousCullMode;
            _graphicsDevice.SetRenderTarget(_shaderManager.CombineFinal);
            
            _shaderManager.DrawFullscreenQuad(_shaderManager.CombineFinalEffect);
            _shaderManager.DrawFullscreenQuad(_shaderManager.SkyEffect);
            
            _graphicsDevice.SetRenderTarget(null);
            
            // TODO: Unnecessary to draw it twice
            _shaderManager.DrawFullscreenQuad(_shaderManager.CombineFinalEffect);
            _shaderManager.DrawFullscreenQuad(_shaderManager.SkyEffect);
            
            _graphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
