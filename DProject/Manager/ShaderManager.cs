using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using DProject.Game.Component;
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
        
        //Mesh based
        private TerrainEffect _terrainEffect;
        private WaterEffect _waterEffect;
        private DepthEffect _depthEffect;
        private PropEffect _propEffect;
        private ClipMapTerrainEffect _clipMapTerrainEffect;
        
        //Screen based
        private FXAAEffect _fxaaEffect;

        //Rendertargets
        public RenderTarget2D DepthBuffer;
        public RenderTarget2D ReflectionBuffer;
        public RenderTarget2D RefractionBuffer;
        public RenderTarget2D PreFinalBuffer;
        
        public enum RenderTarget { Depth, Reflection, Refraction, Final }

        public RenderTarget CurrentRenderTarget;

        private GraphicsDevice _graphicsDevice;

        public ShaderManager()
        {
            _effects = new List<Effect>();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            CreateBuffers(graphicsDevice, false);
        }

        public void CreateBuffers(GraphicsDevice graphicsDevice, bool updateShaders)
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
            
            PreFinalBuffer = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            if(updateShaders)
                SetInitiateShaderInfo();
        }

        public void LoadContent(ContentManager content)
        {
            _waterEffect = new WaterEffect(content.Load<Effect>("shaders/WaterShader"));
            _depthEffect = new DepthEffect(content.Load<Effect>("shaders/DepthShader"));
            _terrainEffect = new TerrainEffect(content.Load<Effect>("shaders/TerrainShader"));
            _propEffect = new PropEffect(content.Load<Effect>("shaders/PropShader"));
            _clipMapTerrainEffect = new ClipMapTerrainEffect(content.Load<Effect>("shaders/ClipMapTerrainShader"));
            
            _fxaaEffect = new FXAAEffect(content.Load<Effect>("shaders/FXAAShader"));
            
            _effects.Add(_depthEffect);
            _effects.Add(_terrainEffect);
            _effects.Add(_propEffect);
            _effects.Add(_waterEffect);
            _effects.Add(_clipMapTerrainEffect);
            _effects.Add(_fxaaEffect);
            
            _clipMapTerrainEffect.Diffuse = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-diffuse.png"), _graphicsDevice);
            _clipMapTerrainEffect.Heightmap = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-height.png"), _graphicsDevice);
            _clipMapTerrainEffect.NormalMap = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-normal.png"), _graphicsDevice);
            _waterEffect.DuDvTexture = content.Load<Texture2D>("shaders/water_dudv");
            
            SetInitiateShaderInfo();
        }

        //TODO: This method is temporary until it will be replaces by a proper shader-information handler.
        public void SetContinuousShaderInfo(LensComponent lens, float relativeGameTime)
        {
            SetContinuousShaderInfo(lens.View, lens.Projection, lens.ReflectionView, lens.Position, relativeGameTime, lens.ReflectionPlaneHeight, lens.NearPlaneDistance, lens.FarPlaneDistance);
        }
        
        public void SetContinuousShaderInfo(Matrix view, Matrix projection, Matrix reflectionView, Vector3 cameraPosition, float relativeGameTime, float waterHeight, float nearPlaneDistance, float farPlaneDistance)
        {
            foreach (var effect in _effects)
            {
                if (effect is AbstractEffect abstractEffect && effect.GetType() != typeof(FXAAEffect))
                {
                    abstractEffect.View = view;
                    abstractEffect.Projection = projection;
                }

                if (effect is IReflected reflectedEffect)
                {
                    reflectedEffect.ReflectionView = reflectionView;
                    reflectedEffect.WaterHeight = waterHeight;
                }
                
                if (effect is IReflective reflectiveEffect)
                {
                    reflectiveEffect.CameraPosition = cameraPosition;
                    reflectiveEffect.ReflectionBuffer = ReflectionBuffer;
                    reflectiveEffect.RefractionBuffer = RefractionBuffer;
                }

                if (effect is INeedClipPlanes clipPlanedEffect)
                {
                    clipPlanedEffect.NearClipPlane = nearPlaneDistance;
                    clipPlanedEffect.FarClipPlane = farPlaneDistance;
                }
            }

            _waterEffect.DepthBuffer = DepthBuffer;
            _waterEffect.RelativeGameTime = relativeGameTime;
        }
        
        //TODO: This method is temporary until it will be replaces by a proper shader-information handler.
        public void SetInitiateShaderInfo()
        {
            //Water
            _waterEffect.MaxWaterDepth = 50f;
            _waterEffect.DuDvTiling = 20.0f;
            _waterEffect.DistortionIntensity = 0.03f;
            _waterEffect.FresnelIntensity = 2.0f;
            _waterEffect.WaterSpeed = 8.0f;
            _waterEffect.WaterColor = new Vector3(0.12f, 0.19f, 0.37f);
            _waterEffect.DeepWaterColor = new Vector3(0f, 1f, 1f);
            _waterEffect.MinimumFoamDistance = 0.05f;
            _waterEffect.MaximumFoamDistance = 0.1f;
            
            //FXAA
            var viewport = new Viewport(0, 0, PreFinalBuffer.Width, PreFinalBuffer.Height);
            var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            var halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            
            _fxaaEffect.World = Matrix.Identity;
            _fxaaEffect.View = Matrix.Identity;
            _fxaaEffect.Projection = halfPixelOffset * projection;
            _fxaaEffect.InverseViewportSize = new Vector2(1f / viewport.Width, 1f / viewport.Height);

            _fxaaEffect.SubPixelAliasingRemoval = 0.75f;
            _fxaaEffect.EdgeThreshold = 0.166f;
            _fxaaEffect.EdgeThresholdMin = 0f;
            _fxaaEffect.CurrentTechnique = _fxaaEffect.Techniques["FXAA"];

            _clipMapTerrainEffect.TextureDimension = new Vector2(1024f, 1024f);
            _clipMapTerrainEffect.ClipMapOffset = new Vector2(512f,512f);
            _clipMapTerrainEffect.ClipMapScale = 0.125f;
        }

        //Mesh based
        public TerrainEffect TerrainEffect => _terrainEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");
        public WaterEffect WaterEffect => _waterEffect ?? throw new ContentLoadException("The WaterEffect shader has not been loaded yet.");
        public DepthEffect DepthEffect => _depthEffect ?? throw new ContentLoadException("The DepthEffect shader has not been loaded yet.");
        public PropEffect PropEffect => _propEffect ?? throw new ContentLoadException("The TerrainEffect shader has not been loaded yet.");
        public ClipMapTerrainEffect ClipMapTerrainEffect => _clipMapTerrainEffect ?? throw new ContentLoadException("The ClipMapTerrainEffect shader has not been loaded yet.");
        
        //Screen based
        public FXAAEffect FXAAEffect => _fxaaEffect ?? throw new ContentLoadException("The FXAAEffect shader has not been loaded yet.");
        
        private static Texture2D ConvertToTexture(Bitmap bitmap, GraphicsDevice graphicsDevice)
        {
            Texture2D texture2D;
            
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                texture2D = Texture2D.FromStream(graphicsDevice, memoryStream);
            }
            
            return texture2D;
        }
    }
} 
