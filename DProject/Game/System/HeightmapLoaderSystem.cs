using System;
using System.Linq;
using System.Threading.Tasks;
using DProject.Game.Component;
using DProject.List;
using DProject.Type.Rendering;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class HeightmapLoaderSystem : EntityUpdateSystem
    {
        public const float IncrementsPerHeightUnit = 256f;
        
        private ComponentMapper<HeightmapComponent> _heightmapMapper;
        private ComponentMapper<LoadedHeightmapComponent> _loadedHeightmapMapper;

        public HeightmapLoaderSystem() : base(Aspect.All(typeof(HeightmapComponent))) { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _heightmapMapper = mapperService.GetMapper<HeightmapComponent>();
            _loadedHeightmapMapper = mapperService.GetMapper<LoadedHeightmapComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            Parallel.ForEach(ActiveEntities, entity =>
            {
                var heightmap = _heightmapMapper.Get(entity);

                var loadedHeightmapComponent = new LoadedHeightmapComponent();
                LoadHeightmap(heightmap.Heightmap, loadedHeightmapComponent);

                _loadedHeightmapMapper.Put(entity, loadedHeightmapComponent);
                _heightmapMapper.Delete(entity);
            });
        }
        
        private static void LoadHeightmap(Vertex[,] heightmap, LoadedHeightmapComponent loadedHeightmapComponent)
        {
            loadedHeightmapComponent.Size = new Point(heightmap.GetLength(0), heightmap.GetLength(1));
            
            loadedHeightmapComponent.LowestHeightValue = ushort.MaxValue;
            loadedHeightmapComponent.HighestHeightValue = ushort.MinValue;

            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            var normals = GenerateNormalMap(heightmap, loadedHeightmapComponent.Size);
            loadedHeightmapComponent.PrimitiveCount = loadedHeightmapComponent.Size.X * loadedHeightmapComponent.Size.Y * 2;
            
            var vertexPositions = new VertexPositionTextureColorNormal[loadedHeightmapComponent.PrimitiveCount * 3];
            var vertexIndex = 0;
            
            for (var x = 0; x < width - 1; x++)
            {
                for (var y = 0; y < height - 1; y++)
                {
                    var vertexTextureId = heightmap[x,y].TextureId;
                    
                    if (vertexTextureId != null)
                    {
                        var topLeft = new Vector3(x,  heightmap[x,y].Height / IncrementsPerHeightUnit, y);
                        var topRight = new Vector3(x + 1,  heightmap[x + 1, y].Height / IncrementsPerHeightUnit, y);
                        var bottomLeft = new Vector3(x, heightmap[x, y + 1].Height / IncrementsPerHeightUnit, y + 1);
                        var bottomRight = new Vector3(x + 1, heightmap[x + 1, y + 1].Height / IncrementsPerHeightUnit, y + 1);

                        if (heightmap[x, y].Height < loadedHeightmapComponent.LowestHeightValue)
                            loadedHeightmapComponent.LowestHeightValue = heightmap[x, y].Height;
                        if (heightmap[x, y].Height > loadedHeightmapComponent.HighestHeightValue)
                            loadedHeightmapComponent.HighestHeightValue = heightmap[x, y].Height;
                    
                        var colorTopLeft = Colors.ColorList[heightmap[x, y].ColorId].Color;
                        var colorTopRight = Colors.ColorList[heightmap[x + 1, y].ColorId].Color;
                        var colorBottomLeft = Colors.ColorList[heightmap[x, y + 1].ColorId].Color;
                        var colorBottomRight = Colors.ColorList[heightmap[x + 1, y + 1].ColorId].Color;
                            
                        Vector3 normal;

                        var texturePositionTexture = Textures.AtlasList["floor_textures"]
                            .TextureList[Textures.FloorTextureIdentifiers[(ushort) vertexTextureId]]
                            .GetAdjustedTexturePosition();

                        var normalIndexX = x;
                        var normalIndexY = y;
                        
                        if (IsAlternativeDiagonal(topLeft, topRight, bottomRight, bottomLeft))
                        {
                            normal = normals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                            normal = normals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = normals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                        
                            normal = normals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = normals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                            normal = normals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                        }
                        else
                        {
                            normal = normals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                            normal = normals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                            normal = normals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                        
                            normal = normals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                            normal = normals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = normals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                        }
                    }
                    else
                    {
                        loadedHeightmapComponent.PrimitiveCount -= 2;
                    }
                }
            }
            
            Array.Resize(ref vertexPositions, loadedHeightmapComponent.PrimitiveCount * 3);
            
            loadedHeightmapComponent.Vertices = vertexPositions;
        }

        private static Vector3[,] GenerateNormalMap(Vertex[,] heightMap, Point size)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            var normalMap = new Vector3[width, height];
            
            const float yScale = 2f;
            float xzScale = IncrementsPerHeightUnit;
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var normalIndexX = x;
                    var normalIndexY = y;
                    
                    float sx = heightMap[x < size.X - 1 ? x+1 : x, y].Height - heightMap[x != 0 ? x-1 : x, y].Height;
                    if (x == 0 || x == size.X - 1)
                        sx *= 2;

                    float sy = heightMap[x, y<size.Y - 1 ? y+1 : y].Height - heightMap[x, y != 0 ?  y-1 : y].Height;
                    if (y == 0 || y == size.Y - 1)
                        sy *= 2;

                    normalMap[normalIndexX, normalIndexY] = new Vector3(-sx * yScale, 2 * xzScale, sy * yScale);
                    normalMap[normalIndexX, normalIndexY].Normalize();
                }
            }

            return normalMap;
        }
        
        public static Vertex[,] GenerateVertexMap(ushort[,] heightMap, ushort[,] splatMap = null)
        {
            var width = heightMap.GetLength(0);
            var length = heightMap.GetLength(1);
            
            var tempHeightMap = new Vertex[width, length];
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < length; y++)
                {
                    var height = heightMap[x,y];
                    ushort? textureId = Textures.FloorTextureIdentifiers.First().Key;
                    var color = Colors.GetDefaultColorId();

                    if (splatMap != null)
                        color = splatMap[x,y];
                    else
                    {
                        if (height < 2)
                            color = 6;
                        else if (height < 8)
                            color = 7;
                        else
                            color = 8;
                    }

                    tempHeightMap[x,y] = new Vertex()
                    {
                        Height = height,
                        TextureId = textureId,
                        ColorId = color
                    };
                }
            }

            return tempHeightMap;
        }
        
        private static bool IsAlternativeDiagonal(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Distance(a, c) < Vector3.Distance(b, d);
        }
    }
}
