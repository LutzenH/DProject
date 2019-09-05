using DProject.Game.Component.Terrain.ClipMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Terrain.ClipMap
{
    public class ClipMapRenderSystem : EntityDrawSystem
    {
        public static RenderTarget2D[] VisibleRenderTarget;
        
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ShaderManager _shaderManager;

        private ComponentMapper<LoadedClipMapComponent> _loadedClipMapMapper;

        private SpriteBatch _spriteBatch;

        private int positionx;
        private int positiony;

        private static RasterizerState _rasterizerState;
        
        public ClipMapRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.All(typeof(LoadedClipMapComponent)))
        {
            _shaderManager = shaderManager;
            _graphicsDevice = graphicsDevice;
            
            VisibleRenderTarget = new RenderTarget2D[TerrainRenderSystem.ClipMapLevels];
            
            _spriteBatch = new SpriteBatch(graphicsDevice);
            
            _rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace,
                ScissorTestEnable = true
            };
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _loadedClipMapMapper = mapperService.GetMapper<LoadedClipMapComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (_shaderManager.CurrentRenderTarget != ShaderManager.RenderTarget.ClipMap)
                return;

            foreach (var entity in ActiveEntities)
            {
                var clipMap = _loadedClipMapMapper.Get(entity);

                var previousRenderTarget = _graphicsDevice.GetRenderTargets();

                _shaderManager.ClipMapEffect.ViewportSize = clipMap.ViewportSize;
                _shaderManager.ClipMapEffect.TerrainTileTextureSize = new Vector2(256, 256);

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                    positionx--;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                    positionx++;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
                    positiony--;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                    positiony++;

                for (var l = TerrainRenderSystem.ClipMapLevels - 1; l >= 0; l--)
                {
                    var tileSize = ClipMapTerrainMeshLoaderSystem.MeshSize * 4 * (l+1);
                    
                    _graphicsDevice.SetRenderTarget(clipMap.ClipMap[l]);
                    
                    _shaderManager.ClipMapEffect.TerrainTileOrigin = new Vector2(0, 0);
                    _shaderManager.ClipMapEffect.TerrainTileSize = new Vector2(tileSize);
                    _shaderManager.ClipMapEffect.TerrainTileTexture = ClipMapTileLoaderSystem.GetFirstTextureInClipMapLevel(l, ClipMapType.Diffuse);
                    //_shaderManager.ClipMapEffect.TerrainClipMapLevel = 0;
                    
                    var effect = _shaderManager.ClipMapEffect;
                    
                    DrawClipMapQuad(_graphicsDevice, new Vector2(0, 0), new Vector2(256, 256), effect);
                    
                    VisibleRenderTarget[l] = clipMap.ClipMap[l];
                }
                
                _graphicsDevice.SetRenderTargets(previousRenderTarget);
            }
        }

        private static void DrawClipMapQuad(GraphicsDevice graphicsDevice, Vector2 position, Vector2 size, Effect effect)
        {
            var previousRasterizer = graphicsDevice.RasterizerState;

            graphicsDevice.RasterizerState = _rasterizerState;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                        
                var pos3 = new Vector3(position.X, position.Y, 0);
                
                var vertices = new[]
                {
                    new VertexPositionTexture(new Vector3(size.X, 0, 0) + pos3, new Vector2(size.X, 0) + position),
                    new VertexPositionTexture(new Vector3(0, 0, 0) + pos3, Vector2.Zero + position),
                    new VertexPositionTexture(new Vector3(0, size.Y, 0) + pos3, new Vector2(0, size.Y) + position),
                    new VertexPositionTexture(new Vector3(size.X, size.Y, 0) + pos3, size + position)
                };
            
                int[] indices = { 0, 1, 2, 2, 3, 0 };

                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
            }

            graphicsDevice.RasterizerState = previousRasterizer;
        }
    }
}
