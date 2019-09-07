using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using DProject.Game.Component;
using DProject.Type.Rendering;
using DProject.Type.Rendering.Primitives;
using DProject.Type.Rendering.Shaders;
using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager
{
    public class ShaderManager
    {
        private GraphicsDevice _graphicsDevice;

        private ClipMapTerrainEffect _clipMapTerrainEffect;
        private GBufferEffect _gBufferEffect;
        
        private ClearGBufferEffect _clearGBufferEffect;
        private DirectionalLightEffect _directionalLightEffect;
        private PointLightEffect _pointLightEffect;
        private CombineFinalEffect _combineFinalEffect;

        // Render-targets (G-Buffer)
        private readonly RenderTargetBinding[] _renderTargetBindings = new RenderTargetBinding[3];
        
        // Color + Specular Intensity
        public RenderTarget2D Color;
        
        // Normal + Specular Power.
        public RenderTarget2D Normal; 
        
        // Depth
        public RenderTarget2D Depth;
        
        // Lights
        public RenderTarget2D Lights;
        
        private FullscreenQuad _fullscreenQuad;
        private Primitives _primitives;

        public ShaderManager()
        {
            _primitives = new Primitives();
            //_effects = new List<Effect>();
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

            _renderTargetBindings[0] = new RenderTargetBinding(Color);
            _renderTargetBindings[1] = new RenderTargetBinding(Normal);
            _renderTargetBindings[2] = new RenderTargetBinding(Depth);

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
            _clipMapTerrainEffect = new ClipMapTerrainEffect(content.Load<Effect>("shaders/ClipMapTerrainShader"));
            _gBufferEffect = new GBufferEffect(content.Load<Effect>("shaders/gbuffer/RenderGBuffer"));
            _clearGBufferEffect = new ClearGBufferEffect(content.Load<Effect>("shaders/gbuffer/ClearGBuffer"));
            _directionalLightEffect = new DirectionalLightEffect(content.Load<Effect>("shaders/gbuffer/DirectionalLight"));
            _pointLightEffect = new PointLightEffect(content.Load<Effect>("shaders/gbuffer/PointLight"));
            _combineFinalEffect = new CombineFinalEffect(content.Load<Effect>("shaders/gbuffer/CombineFinal"));

            _clipMapTerrainEffect.Diffuse = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-diffuse.png"), _graphicsDevice);
            _clipMapTerrainEffect.Height = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-height.png"), _graphicsDevice);
            _clipMapTerrainEffect.Normal = ConvertToTexture(new Bitmap(Game1.RootDirectory + "textures/terrain/terrain-normal.png"), _graphicsDevice);
            
            _primitives.LoadPrimitives(content);
            
            SetInitiateShaderInfo();
        }

        // TODO: This method is temporary until it will be replaces by a proper shader-information handler.
        public void SetContinuousShaderInfo(LensComponent lens, float relativeGameTime)
        {
            _clipMapTerrainEffect.View = lens.View;
            _clipMapTerrainEffect.Projection = lens.Projection;
            
            _gBufferEffect.View = lens.View;
            _gBufferEffect.Projection = lens.Projection;

            _directionalLightEffect.CameraPosition = lens.Position;
            _directionalLightEffect.ColorMap = Color;
            _directionalLightEffect.NormalMap = Normal;
            _directionalLightEffect.DepthMap = Depth;
            _directionalLightEffect.InvertViewProjection = Matrix.Invert(lens.View * lens.Projection);

            _pointLightEffect.View = lens.View;
            _pointLightEffect.Projection = lens.Projection;
            _pointLightEffect.CameraPosition = lens.Position;
            _pointLightEffect.InvertViewProjection = Matrix.Invert(lens.View * lens.Projection);
            _pointLightEffect.ColorMap = Color;
            _pointLightEffect.DepthMap = Depth;
            _pointLightEffect.NormalMap = Normal;

            _combineFinalEffect.ColorMap = Color;
            _combineFinalEffect.LightMap = Lights;
        }
        
        //TODO: This method is temporary until it will be replaces by a proper shader-information handler.
        public void SetInitiateShaderInfo()
        {
            _clipMapTerrainEffect.SpecularIntensity = 0.0f;
            _clipMapTerrainEffect.SpecularPower = 0.0f;
            
            _gBufferEffect.SpecularIntensity = 0.8f;
            _gBufferEffect.SpecularPower = 0.5f;
            
            _clipMapTerrainEffect.TextureDimension = new Vector2(4096f, 4096f);
            _clipMapTerrainEffect.ClipMapOffset = new Vector2(-2048f, -2048f);
            _clipMapTerrainEffect.ClipMapScale = 1.0f;
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
        
        public ClipMapTerrainEffect ClipMapTerrainEffect => _clipMapTerrainEffect ?? throw new ContentLoadException("The ClipMapTerrainEffect shader has not been loaded yet.");
        
        public GBufferEffect GBufferEffect => _gBufferEffect ?? throw new ContentLoadException("The GBufferEffect shader has not been loaded yet.");
        
        public DirectionalLightEffect DirectionalLightEffect => _directionalLightEffect ?? throw new ContentLoadException("The DirectionalLightEffect shader has not been loaded yet.");

        public PointLightEffect PointLightEffect => _pointLightEffect ?? throw new ContentLoadException("The PointLightEffect shader has not been loaded yet.");
        
        public CombineFinalEffect CombineFinalEffect => _combineFinalEffect ?? throw new ContentLoadException("The CombineFinalEffect shader has not been loaded yet.");

        #endregion
        
        //TODO: Remove when ClipMapping is finished.
        private static Texture2D ConvertToTexture(Bitmap bitmap, GraphicsDevice graphicsDevice)
        {
            if (bitmap == null)
                return null;
            
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
