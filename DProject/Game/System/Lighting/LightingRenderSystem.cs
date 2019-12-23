using DProject.Game;
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

        private ComponentMapper<DirectionalLightComponent> _directionalLightMapper;
        private ComponentMapper<PointLightComponent> _pointLightMapper;
        
        public LightingRenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.One(typeof(DirectionalLightComponent), typeof(PointLightComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _directionalLightMapper = mapperService.GetMapper<DirectionalLightComponent>();
            _pointLightMapper = mapperService.GetMapper<PointLightComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            var previousCullMode = _graphicsDevice.RasterizerState.CullMode;

            if (GraphicsManager.Instance.EnableLights)
            {
                _graphicsDevice.SetRenderTarget(ShaderManager.Instance.Lights);
                _graphicsDevice.Clear(Color.Transparent);
                
                _graphicsDevice.BlendState = BlendState.AlphaBlend;
                _graphicsDevice.BlendState.AlphaBlendFunction = BlendFunction.Add;
                _graphicsDevice.BlendState.AlphaSourceBlend = Blend.One;
                _graphicsDevice.BlendState.AlphaDestinationBlend = Blend.One;
                
                foreach (var entity in ActiveEntities)
                {
                    if (_directionalLightMapper.Has(entity))
                    {
                        var light = _directionalLightMapper.Get(entity);

                        ShaderManager.Instance.DirectionalLightEffect.LightDirection = light.Direction;
                        ShaderManager.Instance.DirectionalLightEffect.LightColor = light.Color.ToVector3();
                    
                        ShaderManager.Instance.DrawFullscreenQuad(ShaderManager.Instance.DirectionalLightEffect);
                    }

                    if (_pointLightMapper.Has(entity))
                    {
                        var light = _pointLightMapper.Get(entity);

                        ShaderManager.Instance.PointLightEffect.LightColor = light.Color.ToVector3();
                        ShaderManager.Instance.PointLightEffect.LightIntensity = light.Intensity;
                        ShaderManager.Instance.PointLightEffect.LightPosition = light.WorldPosition;
                        ShaderManager.Instance.PointLightEffect.LightRadius = light.Radius;
                        ShaderManager.Instance.PointLightEffect.World = light.WorldMatrix;

                        var cameraToCenter = Vector3.Distance(CameraSystem.ActiveLens.Position, light.WorldPosition);
                        _graphicsDevice.RasterizerState.CullMode = cameraToCenter <= light.Radius ? CullMode.CullClockwiseFace : CullMode.CullCounterClockwiseFace;

                        _graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
                        ShaderManager.Instance.DrawPrimitive(ShaderManager.Instance.PointLightEffect, PrimitiveType.Sphere);
                    }
                }
            }

            if (GraphicsManager.Instance.EnableSSAO)
            {
                _graphicsDevice.RasterizerState.CullMode = previousCullMode;
                _graphicsDevice.SetRenderTarget(ShaderManager.Instance.SSAO);
                ShaderManager.Instance.DrawFullscreenQuad(ShaderManager.Instance.SSAOEffect);
                _graphicsDevice.BlendState = BlendState.Opaque;   
            }

            _graphicsDevice.RasterizerState.CullMode = previousCullMode;
            _graphicsDevice.SetRenderTarget(ShaderManager.Instance.CombineFinal);
            _graphicsDevice.BlendState = BlendState.Opaque;
            ShaderManager.Instance.DrawFullscreenQuad(ShaderManager.Instance.CombineFinalEffect);
            
            if (GraphicsManager.Instance.EnableSky)
                ShaderManager.Instance.DrawFullscreenQuad(ShaderManager.Instance.SkyEffect);
        }
    }
}
