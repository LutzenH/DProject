using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using DProject.Game.Component;
using DProject.Type.Rendering;
using DProject.Type.Rendering.Primitives;
using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager
{
    public class ShaderManager
    {
        private GraphicsDevice _graphicsDevice;

        // GBuffer Effects
        private GBufferEffect _gBufferEffect;
        private ClearGBufferEffect _clearGBufferEffect;
        private DirectionalLightEffect _directionalLightEffect;
        private PointLightEffect _pointLightEffect;
        private CombineFinalEffect _combineFinalEffect;
        
        // Water Effect
        private WaterEffect _waterEffect;
        
        // Skybox Effect
        private SkyEffect _skyEffect;
        
        // FXAA Effect
        private FXAAEffect _fxaaEffect;
        
        // SSAO Effect
        private SSAOEffect _ssaoEffect;

        // Render-targets (G-Buffer)
        private readonly RenderTargetBinding[] _renderTargetBindings = new RenderTargetBinding[4];
        
        // Color
        public RenderTarget2D Color;
        // Normal
        public RenderTarget2D Normal;
        // Specular Intensity + Specular Power + Emission
        public RenderTarget2D LightInfo;
        // Depth
        public RenderTarget2D Depth;
        // Lights
        public RenderTarget2D Lights;
        // SSAO
        public RenderTarget2D SSAO;
        // Final
        public RenderTarget2D CombineFinal;
        
        private FullscreenQuad _fullscreenQuad;
        private Primitives _primitives;

        public ShaderManager()
        {
            _primitives = new Primitives();
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _fullscreenQuad = new FullscreenQuad(graphicsDevice);
            _primitives.Initialize(graphicsDevice);
            
            CreateGBuffer(false);
        }

        public void CreateGBuffer(bool updateShaders)
        {
            Color = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24);
            
            Normal = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24);
            
            LightInfo = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24);
            
            Depth = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24);
            
            Lights = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24);
            
            SSAO = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24);
            
            CombineFinal = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24);

            _renderTargetBindings[0] = new RenderTargetBinding(Color);
            _renderTargetBindings[1] = new RenderTargetBinding(Normal);
            _renderTargetBindings[2] = new RenderTargetBinding(Depth);
            _renderTargetBindings[3] = new RenderTargetBinding(LightInfo);

            if(updateShaders)
                SetInitiateShaderInfo();
        }

        public void SetGBuffer()
        {
            _graphicsDevice.SetRenderTargets(_renderTargetBindings);
        }

        public void ClearGBuffer()
        {
            DrawFullscreenQuad(ClearBufferEffect);
        }
        
        public void ResolveGBuffer()
        {
            _graphicsDevice.SetRenderTargets(null);
        }

        public void LoadContent(ContentManager content)
        {
            _gBufferEffect = new GBufferEffect(content.Load<Effect>("shaders/gbuffer/RenderGBuffer"));
            _clearGBufferEffect = new ClearGBufferEffect(content.Load<Effect>("shaders/gbuffer/ClearGBuffer"));
            _directionalLightEffect = new DirectionalLightEffect(content.Load<Effect>("shaders/gbuffer/DirectionalLight"));
            _pointLightEffect = new PointLightEffect(content.Load<Effect>("shaders/gbuffer/PointLight"));
            _combineFinalEffect = new CombineFinalEffect(content.Load<Effect>("shaders/gbuffer/CombineFinal"));

            _waterEffect = new WaterEffect(content.Load<Effect>("shaders/WaterShader"));
            _waterEffect.DuDvTexture = content.Load<Texture2D>("shaders/water_dudv");
            
            _skyEffect = new SkyEffect(content.Load<Effect>("shaders/SkyShader"));
            
            _fxaaEffect = new FXAAEffect(content.Load<Effect>("shaders/FXAAShader"));
            _ssaoEffect = new SSAOEffect(content.Load<Effect>("shaders/SSAOShader"));
            _ssaoEffect.Noise = content.Load<Texture2D>("shaders/noise");

            _primitives.LoadPrimitives(content);
            
            SetInitiateShaderInfo();
        }

        // TODO: This method is temporary until it will be replaces by a proper shader-information handler.
        public void SetContinuousShaderInfo(LensComponent lens, float relativeGameTime)
        {
            _gBufferEffect.View = lens.View;
            _gBufferEffect.Projection = lens.Projection;

            _directionalLightEffect.CameraPosition = lens.Position;
            _directionalLightEffect.LightInfoMap = LightInfo;
            _directionalLightEffect.NormalMap = Normal;
            _directionalLightEffect.DepthMap = Depth;
            _directionalLightEffect.InvertViewProjection = Matrix.Invert(lens.View * lens.Projection);

            _pointLightEffect.View = lens.View;
            _pointLightEffect.Projection = lens.Projection;
            _pointLightEffect.CameraPosition = lens.Position;
            _pointLightEffect.InvertViewProjection = Matrix.Invert(lens.View * lens.Projection);
            _pointLightEffect.LightInfoMap = LightInfo;
            _pointLightEffect.DepthMap = Depth;
            _pointLightEffect.NormalMap = Normal;

            _combineFinalEffect.ColorMap = Color;
            _combineFinalEffect.LightMap = Lights;
            _combineFinalEffect.LightInfoMap = LightInfo;
            _combineFinalEffect.SSAOMap = SSAO;

            _waterEffect.RefractionBuffer = CombineFinal;
            _waterEffect.ReflectionBuffer = CombineFinal;
            _waterEffect.DepthBuffer = Depth;
            _waterEffect.RelativeGameTime = relativeGameTime;
            _waterEffect.View = lens.View;
            _waterEffect.Projection = lens.Projection;
            _waterEffect.NearClipPlane = lens.NearPlaneDistance;
            _waterEffect.FarClipPlane = lens.FarPlaneDistance;
            _waterEffect.CameraPosition = lens.Position;

            _ssaoEffect.NormalMap = Normal;
            _ssaoEffect.DepthMap = Depth;
            
            _skyEffect.Depth = Depth;
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
            
            _skyEffect.ViewportResolution = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            
            _fxaaEffect.World = Matrix.Identity;
            _fxaaEffect.View = Matrix.Identity;
            
            var viewport = new Viewport(0, 0, CombineFinal.Width, CombineFinal.Height);
            var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);

            _fxaaEffect.Projection = projection;
            _fxaaEffect.InverseViewportSize = new Vector2(1f / viewport.Width, 1f / viewport.Height);
            
            _fxaaEffect.SubPixelAliasingRemoval = 0.75f;
            _fxaaEffect.EdgeThreshold = 0.166f;
            _fxaaEffect.EdgeThresholdMin = 0f;
            _fxaaEffect.CurrentTechnique = _fxaaEffect.Techniques["FXAA"];

            _ssaoEffect.TotalStrength = 0.5f;
            _ssaoEffect.Strength = 0.07f;
            _ssaoEffect.Offset = 18.0f;
            _ssaoEffect.Falloff = 0.25f;
            _ssaoEffect.Rad = 0.003f;
        }

        #region Draw Primitives

        public void DrawFullscreenQuad(Effect effect)
        {
            _fullscreenQuad.Draw(effect);
        }

        public void DrawPrimitive(Effect effect, PrimitiveType type)
        {
            _primitives.Draw(effect, type);
        }
        
        #endregion

        #region GBuffer Effects

        public ClearGBufferEffect ClearBufferEffect => _clearGBufferEffect ?? throw new ContentLoadException("The ClearGBufferEffect shader has not been loaded yet.");
        
        public GBufferEffect GBufferEffect => _gBufferEffect ?? throw new ContentLoadException("The GBufferEffect shader has not been loaded yet.");
        
        public DirectionalLightEffect DirectionalLightEffect => _directionalLightEffect ?? throw new ContentLoadException("The DirectionalLightEffect shader has not been loaded yet.");

        public PointLightEffect PointLightEffect => _pointLightEffect ?? throw new ContentLoadException("The PointLightEffect shader has not been loaded yet.");
        
        public CombineFinalEffect CombineFinalEffect => _combineFinalEffect ?? throw new ContentLoadException("The CombineFinalEffect shader has not been loaded yet.");

        #endregion

        #region Other Effects

        public WaterEffect WaterEffect => _waterEffect ?? throw new ContentLoadException("The WaterEffect shader has not been loaded yet.");

        public SkyEffect SkyEffect => _skyEffect ?? throw new ContentLoadException("The SkyboxEffect shader has not been loaded yet.");
        
        public FXAAEffect FXAAEffect => _fxaaEffect ?? throw new ContentLoadException("The FXAAEffect shader has not been loaded yet.");

        public SSAOEffect SSAOEffect => _ssaoEffect ?? throw new ContentLoadException("The SSAOEffect shader has not been loaded yet.");

        #endregion
    }
} 
