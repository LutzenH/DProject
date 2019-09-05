using System;
using DProject.Game.Component;
using DProject.Game.Component.Terrain;
using DProject.Type.Rendering.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Terrain
{
    public class TerrainRenderSystem : EntityDrawSystem
    {
        public const int ClipMapLevels = 6;
        
        private static readonly Matrix[] Rotations = {
            Matrix.CreateRotationY(MathHelper.ToRadians(0)),
            Matrix.CreateRotationY(MathHelper.ToRadians(90f)),
            Matrix.CreateRotationY(MathHelper.ToRadians(270f)),
            Matrix.CreateRotationY(MathHelper.ToRadians(180))
        };

        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
        
        private ComponentMapper<LoadedClipMapTerrainComponent> _loadedClipMapTerrainMapper;

        private Vector3 _cameraPosition;
        
        public TerrainRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.All(typeof(LoadedClipMapTerrainComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _shaderManager = shaderManager;

            _cameraPosition = Vector3.Zero;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _loadedClipMapTerrainMapper = mapperService.GetMapper<LoadedClipMapTerrainComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                _cameraPosition = CameraSystem.ActiveLens.Position;

            foreach (var entity in ActiveEntities)
            {
                var terrain = _loadedClipMapTerrainMapper.Get(entity);
                
                AbstractEffect effect;

                switch (_shaderManager.CurrentRenderTarget)
                {
                    case ShaderManager.RenderTarget.Depth:
                        effect = _shaderManager.ClipMapTerrainEffect;
                        effect.CurrentTechnique = _shaderManager.ClipMapTerrainEffect.Techniques["Depth"];
                        break;
                    case ShaderManager.RenderTarget.Reflection:
                        effect = _shaderManager.ClipMapTerrainEffect;
                        effect.CurrentTechnique = _shaderManager.ClipMapTerrainEffect.Techniques["Reflection"];
                        break;
                    case ShaderManager.RenderTarget.Refraction:
                        effect = _shaderManager.ClipMapTerrainEffect;
                        effect.CurrentTechnique = _shaderManager.ClipMapTerrainEffect.Techniques["Refraction"];
                        break;
                    case ShaderManager.RenderTarget.Final:
                        effect = _shaderManager.ClipMapTerrainEffect;
                        effect.CurrentTechnique = _shaderManager.ClipMapTerrainEffect.Techniques["BasicColorDrawing"];
                        break;
                    default:
                        return;
                }
                
                var snappedPosition = new Vector2(
                    (float) Math.Floor(_cameraPosition.X),
                    (float) Math.Floor(_cameraPosition.Z));
                
                effect.World = TransformComponent.CalculateWorldMatrix(new Vector3(snappedPosition.X, 0, snappedPosition.Y), Vector3.One, Quaternion.Identity);
                _graphicsDevice.SetVertexBuffer(terrain.CrossVertexBuffer);
                _graphicsDevice.Indices = terrain.CrossIndexBuffer;

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  terrain.CrossIndexBuffer.IndexCount/3);
                }
                
                for(var l = 0; l < ClipMapLevels; l++ ) {
                    // scale is the unit size for this clipmap level
                    // tile_size is the size of a full tile mesh
                    // snapped_pos is the camera position snapped to this level's resolution
                    // base is the bottom left corner of the bottom left tile
                    float scale = 1 << l;
                    snappedPosition = new Vector2(
                        (float) (Math.Floor(_cameraPosition.X / scale) * scale),
                        (float) (Math.Floor(_cameraPosition.Z / scale) * scale));

                    // draw tiles
                    var tileSize = new Vector2(ClipMapTerrainMeshLoaderSystem.MeshSize << l);
                    var basePosition = snappedPosition - tileSize * 2;

                    for(var x = 0; x < 4; x++ ) {
                        for(var y = 0; y < 4; y++ ) {
                            // draw a 4x4 set of tiles. cut out the middle 2x2 unless we're at the finest level
                            if(l != 0 && ( x == 1 || x == 2 ) && ( y == 1 || y == 2 ))
                                continue;

                            // add space for the filler meshes
                            var fill = new Vector2(x >= 2 ? 1 : 0, y >= 2 ? 1 : 0) * scale;
                            var tileBL = basePosition + new Vector2(x, y) * tileSize + fill;
                            
                            effect.World = TransformComponent.CalculateWorldMatrix(new Vector3(tileBL.X, 0, tileBL.Y), new Vector3(scale,  1, scale), Quaternion.Identity);
                            _graphicsDevice.SetVertexBuffer(terrain.TileVertexBuffer);
                            _graphicsDevice.Indices = terrain.TileIndexBuffer;

                            foreach (var pass in effect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  terrain.TileIndexBuffer.IndexCount/3);
                            }
                        }
                    }
                    
                    effect.World = TransformComponent.CalculateWorldMatrix(new Vector3(snappedPosition.X, 0, snappedPosition.Y), new Vector3(scale, 1, scale), Quaternion.Identity);
                    _graphicsDevice.SetVertexBuffer(terrain.FillerVertexBuffer);
                    _graphicsDevice.Indices = terrain.FillerIndexBuffer;

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  terrain.FillerIndexBuffer.IndexCount/3);
                    }
                    
                    if(l != ClipMapLevels - 1 ) {
                        var nextScale = scale * 2.0f;
                        var nextSnappedPos = new Vector2(
                            (float) (Math.Floor(_cameraPosition.X / nextScale) * nextScale),
                            (float) (Math.Floor(_cameraPosition.Z / nextScale) * nextScale));

                        var nextBase = nextSnappedPos - new Vector2(ClipMapTerrainMeshLoaderSystem.MeshSize << (l + 1));

                        effect.World = TransformComponent.CalculateWorldMatrix(new Vector3(nextBase.X, 0, nextBase.Y), new Vector3(scale, 1, scale), Quaternion.Identity);
                        _graphicsDevice.SetVertexBuffer(terrain.SeamVertexBuffer);
                        _graphicsDevice.Indices = terrain.SeamIndexBuffer;

                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  terrain.SeamIndexBuffer.IndexCount/3);
                        }

                        // +0.5 because the mesh is offset by half a unit to make rotations simpler
                        // and we want it to lie on the grid when we draw it
                        var tileCentre = snappedPosition + new Vector2(scale * 0.5f);

                        var d = new Vector2(_cameraPosition.X, _cameraPosition.Z) - nextSnappedPos;
                        var r = 0;
                        r |= d.X >= scale ? 0 : 2;
                        r |= d.Y >= scale ? 0 : 1;

                        effect.World = Rotations[r] * Matrix.CreateScale(new Vector3(scale, 1, scale)) * Matrix.CreateTranslation(new Vector3(tileCentre.X, 0, tileCentre.Y));
                        _graphicsDevice.SetVertexBuffer(terrain.TrimVertexBuffer);
                        _graphicsDevice.Indices = terrain.TrimIndexBuffer;

                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,  terrain.TrimIndexBuffer.IndexCount/3);
                        }
                    }
                }
            }
        }
    }
}
