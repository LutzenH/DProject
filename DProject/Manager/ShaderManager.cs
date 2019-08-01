using DProject.Entity;
using DProject.Entity.Camera;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Manager
{
    public class ShaderManager
    {
        private static Effect _terrainEffect;
        private static Effect _waterEffect;
        private static Effect _depthEffect;
        private static Effect _propEffect;

        public static RenderTarget2D DepthBuffer;
        public static RenderTarget2D ReflectionBuffer;
        public static RenderTarget2D RefractionBuffer;
        
        public enum RenderTarget { Depth, Reflection, Refraction, Final }

        public static RenderTarget CurrentRenderTarget;

        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            DepthBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            ReflectionBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth/2,
                graphicsDevice.PresentationParameters.BackBufferHeight/2,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            RefractionBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth/2,
                graphicsDevice.PresentationParameters.BackBufferHeight/2,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public static void LoadContent(ContentManager content)
        {
            _waterEffect = content.Load<Effect>("Shaders/WaterShader");
            _depthEffect = content.Load<Effect>("Shaders/DepthShader");
            _terrainEffect = content.Load<Effect>("Shaders/TerrainShader");
            _propEffect = content.Load<Effect>("Shaders/PropShader");
        }

        public static void SetShaderInfo(CameraEntity activeCamera)
        {
            //DepthEffect
            _depthEffect.Parameters["View"].SetValue(activeCamera.GetViewMatrix());
            _depthEffect.Parameters["Projection"].SetValue(activeCamera.GetProjectMatrix());
            _depthEffect.Parameters["FarClip"].SetValue(activeCamera.GetFarPlaneDistance());

            //WaterEffect
            _waterEffect.Parameters["View"].SetValue(activeCamera.GetViewMatrix());
            _waterEffect.Parameters["Projection"].SetValue(activeCamera.GetProjectMatrix());
            _waterEffect.Parameters["CameraPosition"].SetValue(activeCamera.GetPosition());
            _waterEffect.Parameters["reflectionTexture"].SetValue(ReflectionBuffer);
            _waterEffect.Parameters["refractionTexture"].SetValue(RefractionBuffer);

            //TerrainEffect
            _terrainEffect.Parameters["View"].SetValue(activeCamera.GetViewMatrix());
            _terrainEffect.Parameters["ReflectionView"].SetValue(activeCamera.GetReflectionViewMatrix());
            _terrainEffect.Parameters["Projection"].SetValue(activeCamera.GetProjectMatrix());
            _terrainEffect.Parameters["WaterHeight"].SetValue(WaterPlaneEntity.WaterHeight);
            
            //PropEffect
            _propEffect.Parameters["View"].SetValue(activeCamera.GetViewMatrix());
            _propEffect.Parameters["ReflectionView"].SetValue(activeCamera.GetReflectionViewMatrix());
            _propEffect.Parameters["Projection"].SetValue(activeCamera.GetProjectMatrix());
            _propEffect.Parameters["WaterHeight"].SetValue(WaterPlaneEntity.WaterHeight);
        }

        public static Effect TerrainEffect => _terrainEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");

        public static Effect WaterEffect => _waterEffect ?? throw new ContentLoadException("The WaterEffect shader has not been loaded yet.");

        public static Effect DepthEffect => _depthEffect ?? throw new ContentLoadException("The DepthEffect shader has not been loaded yet.");
        
        public static Effect PropEffect => _propEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");
    }
} 
