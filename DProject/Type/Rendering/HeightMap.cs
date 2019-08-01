using System;
using System.Linq;
using DProject.List;
using DProject.Manager;
using DProject.Type.Enum;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class HeightMap
    {
        private VertexPositionTextureColorNormal[] _vertexPositions;
        private Vector3[,] _heightNormals;

        private DynamicVertexBuffer _vertexBuffer;
        
        private LevelOfDetail _levelOfDetail;
        
        private readonly int _width;
        private readonly int _height;

        private GraphicsDevice _graphicsDevice;

        private int _primitiveCount;
        private int _previousVertexCount;
        
        private bool _hasUpdated;
        
        public const float IncrementsPerHeightUnit = 256f;

        private ushort _lowestHeightValue;
        private ushort _highestHeightValue;

        public HeightMap(Vertex[,] heightMap, LevelOfDetail levelOfDetail)
        {
            _width = heightMap.GetLength(0);
            _height = heightMap.GetLength(1);

            _levelOfDetail = levelOfDetail;
            
            UpdateVertexPositions(heightMap);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            if (_vertexBuffer == null || GetVertexCount() != _previousVertexCount)
            {
                _previousVertexCount = GetVertexCount();
                _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionTextureColorNormal), GetVertexCount(), BufferUsage.WriteOnly);
            }

            if(GetVertexCount() != 0)
                _vertexBuffer.SetData(_vertexPositions);
        }

        private void UpdateVertexPositions(Vertex[,] heightMap)
        {
            _vertexPositions = GenerateVertexPositions(heightMap);
        }

        public void UpdateHeightMap(Vertex[,] heightMap)
        {
            UpdateVertexPositions(heightMap);
            Initialize(_graphicsDevice);
            SetHasUpdated(false);
        }

        public void SetLevelOfDetail(Vertex[,] heightMap, LevelOfDetail levelOfDetail)
        {
            SetLevelOfDetail(levelOfDetail);
            UpdateHeightMap(heightMap);
        }

        public void Draw(Matrix worldMatrix, Texture2D texture2D)
        {
            Effect effect = null;
            
            switch (ShaderManager.CurrentRenderTarget)
            {
                case ShaderManager.RenderTarget.Depth:
                    effect = ShaderManager.DepthEffect;
                    effect.Parameters["World"].SetValue(worldMatrix);
                    break;
                case ShaderManager.RenderTarget.Refraction:
                case ShaderManager.RenderTarget.Reflection:
                case ShaderManager.RenderTarget.Final:
                    effect = ShaderManager.TerrainEffect;
                    effect.Parameters["World"].SetValue(worldMatrix);
                    break;
                default:
                    return;
            }
            
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                
                if(GetVertexCount() != 0)
                    _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _primitiveCount);
            }
        }

        private int GetVertexCount()
        {
            //Amount of triangles times 3 (3 vertices per triangle)
            return _primitiveCount * 3;
        }

        private VertexPositionTextureColorNormal[] GenerateVertexPositions(Vertex[,] heightMap)
        {
            _lowestHeightValue = ushort.MaxValue;
            _highestHeightValue = ushort.MinValue;

            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);

            var offset = GetVertexOffset(_levelOfDetail);

            switch (_levelOfDetail)
            {
                case LevelOfDetail.Full:
                    _primitiveCount = _width * _height * 2;
                    break;
                case LevelOfDetail.Half:
                    _primitiveCount = _width * _height;
                    break;
                case LevelOfDetail.Quarter:
                    _primitiveCount = _width * _height / 2;
                    break;
                case LevelOfDetail.Eighth:
                    _primitiveCount = _width * _height / 4;
                    break;
                case LevelOfDetail.Sixteenth:
                    _primitiveCount = _width * _height / 8;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _heightNormals = GenerateNormalMap(heightMap, _levelOfDetail);

            var vertexPositions = new VertexPositionTextureColorNormal[GetVertexCount()];
            var vertexIndex = 0;
            
            for (var x = 0; x < width - 1; x += offset)
            {
                for (var y = 0; y < height - 1; y += offset)
                {
                    var vertexTextureId = heightMap[x,y].TextureId;
                    
                    if (vertexTextureId != null)
                    {
                        var topLeft = new Vector3 (x,  heightMap[x,y].Height / IncrementsPerHeightUnit, y);
                        var topRight = new Vector3 (x + offset,  heightMap[x + offset, y].Height / IncrementsPerHeightUnit, y);
                        var bottomLeft = new Vector3 (x, heightMap[x, y + offset].Height / IncrementsPerHeightUnit, y + offset);
                        var bottomRight = new Vector3 (x + offset, heightMap[x + offset,y + offset].Height / IncrementsPerHeightUnit, y + offset);

                        if (heightMap[x, y].Height < _lowestHeightValue)
                            _lowestHeightValue = heightMap[x, y].Height;
                        if (heightMap[x, y].Height > _highestHeightValue)
                            _highestHeightValue = heightMap[x, y].Height;
                    
                        var colorTopLeft = Colors.ColorList[heightMap[x, y].ColorId].Color;
                        var colorTopRight = Colors.ColorList[heightMap[x + offset, y].ColorId].Color;
                        var colorBottomLeft = Colors.ColorList[heightMap[x, y + offset].ColorId].Color;
                        var colorBottomRight = Colors.ColorList[heightMap[x + offset, y + offset].ColorId].Color;
                            
                        Vector3 normal;

                        var texturePositionTexture = Textures.AtlasList["floor_textures"]
                            .TextureList[Textures.FloorTextureIdentifiers[(ushort) vertexTextureId]]
                            .GetAdjustedTexturePosition();

                        var normalIndexX = x / offset;
                        var normalIndexY = y / offset;
                        
                        if (IsAlternativeDiagonal(topLeft, topRight, bottomRight, bottomLeft))
                        {
                            normal = _heightNormals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                            normal = _heightNormals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = _heightNormals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                        
                            normal = _heightNormals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = _heightNormals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                            normal = _heightNormals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                        }
                        else
                        {
                            normal = _heightNormals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                            normal = _heightNormals[normalIndexX + 1, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.Y));
                            normal = _heightNormals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                        
                            normal = _heightNormals[normalIndexX, normalIndexY + 1];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.Y));
                            normal = _heightNormals[normalIndexX, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture.X,texturePositionTexture.W));
                            normal = _heightNormals[normalIndexX + 1, normalIndexY];
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture.Z,texturePositionTexture.W));
                        }
                    }
                    else
                    {
                        _primitiveCount -= 2;
                    }
                }
            }
            
            Array.Resize(ref vertexPositions, GetVertexCount());

            return vertexPositions;
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

        public Vector3[,] GenerateNormalMap(Vertex[,] heightMap, LevelOfDetail levelOfDetail)
        {
            var offset = GetVertexOffset(levelOfDetail);
            
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            var normalMap = new Vector3[width, height];
            
            const float yScale = 2f;
            float xzScale = IncrementsPerHeightUnit * ((float) levelOfDetail + 1);
            
            for (var x = 0; x < width; x += offset)
            {
                for (var y = 0; y < height; y += offset)
                {
                    var normalIndexX = x / offset;
                    var normalIndexY = y / offset;
                    
                    float sx = heightMap[x < _width - 1 ? x+offset : x, y].Height - heightMap[x != 0 ? x-offset : x, y].Height;
                    if (x == 0 || x == _width - 1)
                        sx *= 2;

                    float sy = heightMap[x, y<_height - 1 ? y+offset : y].Height - heightMap[x, y != 0 ?  y-offset : y].Height;
                    if (y == 0 || y == _height - 1)
                        sy *= 2;

                    normalMap[normalIndexX, normalIndexY] = new Vector3(-sx * yScale, 2 * xzScale, sy * yScale);
                    normalMap[normalIndexX, normalIndexY].Normalize();
                }
            }

            return normalMap;
        }
        
        
        public static int GetVertexOffset(LevelOfDetail levelOfDetail)
        {
            switch (levelOfDetail)
            {
                case LevelOfDetail.Full:
                    return 1;
                case LevelOfDetail.Half:
                    return 2;
                case LevelOfDetail.Quarter:
                    return 4;
                case LevelOfDetail.Eighth:
                    return 8;
                case LevelOfDetail.Sixteenth:
                    return 16;
                default:
                    throw new ArgumentOutOfRangeException(nameof(levelOfDetail), levelOfDetail, null);
            }
        }

        //Can be used the get the normal of a triangle
        private static Vector3 GenerateNormalDirection(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 sum = Vector3.Zero;
            
            var n0 = (vertex2 - vertex1) * 10f;
            var n1 = (vertex3 - vertex2) * 10f;

            var cnorm = Vector3.Cross(n0, n1);
            
            sum += cnorm;

            return Vector3.Normalize(sum);
        }

        public static bool IsAlternativeDiagonal(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Distance(a, c) < Vector3.Distance(b, d);
        }

        public bool GetHasUpdated()
        {
            return _hasUpdated;
        }

        public void SetHasUpdated(bool hasUpdated)
        {
            _hasUpdated = hasUpdated;
        }

        public (ushort, ushort) GetOuterHeightBounds()
        {
            return (_lowestHeightValue, _highestHeightValue);
        }

        public LevelOfDetail GetLevelOfDetail()
        {
            return _levelOfDetail;
        }

        public void SetLevelOfDetail(LevelOfDetail levelOfDetail)
        {
            _levelOfDetail = levelOfDetail;
        }
    }
}
