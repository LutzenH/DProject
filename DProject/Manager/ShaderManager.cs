using System.Collections.Generic;
using DProject.Entity;
using DProject.Entity.Camera;
using DProject.Type.Rendering.Shaders;
using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Manager
{
    public class ShaderManager
    {
        private readonly List<Effect> _effects;
        
        private TerrainEffect _terrainEffect;
        private WaterEffect _waterEffect;
        private DepthEffect _depthEffect;
        private PropEffect _propEffect;

        public RenderTarget2D DepthBuffer;
        public RenderTarget2D ReflectionBuffer;
        public RenderTarget2D RefractionBuffer;
        
        public enum RenderTarget { Depth, Reflection, Refraction, Final }

        public static RenderTarget CurrentRenderTarget;

        public ShaderManager()
        {
            _effects = new List<Effect>();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            DepthBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.HdrBlendable,
                DepthFormat.Depth24);
            
            ReflectionBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            RefractionBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public void LoadContent(ContentManager content)
        {
            _waterEffect = new WaterEffect(content.Load<Effect>("shaders/WaterShader"));
            _depthEffect = new DepthEffect(content.Load<Effect>("shaders/DepthShader"));
            _terrainEffect = new TerrainEffect(content.Load<Effect>("shaders/TerrainShader"));
            _propEffect = new PropEffect(content.Load<Effect>("shaders/PropShader"));
            
            _effects.Add(_depthEffect);
            _effects.Add(_terrainEffect);
            _effects.Add(_propEffect);
            _effects.Add(_waterEffect);
            
            _waterEffect.DuDvTexture = content.Load<Texture2D>("shaders/water_dudv");
        }

        //This method is temporary until it will be replaces by a proper information handler.
        public void SetContinuousShaderInfo(CameraEntity activeCamera, float relativeGameTime)
        {
            SetContinuousShaderInfo(activeCamera.GetViewMatrix(), activeCamera.GetProjectMatrix(), activeCamera.GetReflectionViewMatrix(), activeCamera.GetPosition(), relativeGameTime);
        }
        
        public void SetContinuousShaderInfo(Matrix view, Matrix projection, Matrix reflectionView, Vector3 cameraPosition, float relativeGameTime)
        {
            foreach (var effect in _effects)
            {
                if (effect is AbstractEffect abstractEffect)
                {
                    abstractEffect.View = view;
                    abstractEffect.Projection = projection;
                }

                if (effect is IReflected reflectedEffect)
                {
                    reflectedEffect.ReflectionView = reflectionView;
                }
                
                if (effect is IReflective reflectiveEffect)
                {
                    reflectiveEffect.CameraPosition = cameraPosition;
                    reflectiveEffect.ReflectionBuffer = ReflectionBuffer;
                    reflectiveEffect.RefractionBuffer = RefractionBuffer;
                }
            }

            _waterEffect.DepthBuffer = DepthBuffer;
            _waterEffect.RelativeGameTime = relativeGameTime;
        }
        
        //This method is temporary until it will be replaces by a proper information handler.
        public void SetInitiateShaderInfo(CameraEntity activeCamera)
        {
            SetInitiateShaderInfo(
                activeCamera.GetNearPlaneDistance(),
                activeCamera.GetFarPlaneDistance(),
                WaterPlaneEntity.WaterHeight,
                0.05f,
                20.0f,
                0.03f,
                2.0f,
                8.0f,
                new Vector3(0.12f, 0.19f, 0.37f),
                new Vector3(0f, 1f, 1f),
                0.05f,
                0.1f
                );
        }

        public void SetInitiateShaderInfo(
            float nearPlaneDistance,
            float farPlaneDistance,
            float waterHeight,
            float maxWaterDepth,
            float dudvTiling,
            float distortionIntensity,
            float fresnelIntensity,
            float waterSpeed,
            Vector3 waterColor,
            Vector3 deepWaterColor,
            float minimumFoamDistance,
            float maximumFoamDistance)
        {
            foreach (var effect in _effects)
            {
                if (effect is IReflected reflectedEffect)
                {
                    reflectedEffect.WaterHeight = waterHeight;
                }
            }

            _depthEffect.NearClipPlane = nearPlaneDistance;
            _depthEffect.FarClipPlane = farPlaneDistance;

            _waterEffect.NearClipPlane = nearPlaneDistance;
            _waterEffect.FarClipPlane = farPlaneDistance;
            _waterEffect.MaxWaterDepth = maxWaterDepth;
            _waterEffect.DuDvTiling = dudvTiling;
            _waterEffect.DistortionIntensity = distortionIntensity;
            _waterEffect.FresnelIntensity = fresnelIntensity;
            _waterEffect.WaterSpeed = waterSpeed;
            _waterEffect.WaterColor = waterColor;
            _waterEffect.DeepWaterColor = deepWaterColor;
            _waterEffect.MinimumFoamDistance = minimumFoamDistance;
            _waterEffect.MaximumFoamDistance = maximumFoamDistance;
        }

        public TerrainEffect TerrainEffect => _terrainEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");

        public WaterEffect WaterEffect => _waterEffect ?? throw new ContentLoadException("The WaterEffect shader has not been loaded yet.");

        public DepthEffect DepthEffect => _depthEffect ?? throw new ContentLoadException("The DepthEffect shader has not been loaded yet.");
        
        public PropEffect PropEffect => _propEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");
    }
} 
