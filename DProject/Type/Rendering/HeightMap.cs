using System;
using DProject.List;
using DProject.Type.Rendering.Shaders;
using DProject.Type.Serializable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class HeightMap
    {       
        private VertexPositionTextureColorNormal[] _vertexPositions;

        private DynamicVertexBuffer _vertexBuffer;
        
        private static TerrainEffect _terrainEffect;

        private readonly int _width;
        private readonly int _height;

        private GraphicsDevice _graphicsDevice;

        private int _primitiveCount;

        private bool _hasUpdated;

        private const byte DefaultDistanceBetweenFloors = 32;
        
        public const float IncrementsPerHeightUnit = 4f;

        public HeightMap(Tile[,] tiles)
        {
            _width = tiles.GetLength(0);
            _height = tiles.GetLength(1);
            
            _vertexPositions = GenerateVertexPositions(tiles);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            if (_terrainEffect == null)
            {
                //Graphics Effects
                _terrainEffect = new TerrainEffect(graphicsDevice);
                _terrainEffect.Alpha = 1.0f;
            
                _terrainEffect.LightingEnabled = true;
                _terrainEffect.AmbientLightColor = new Vector3(0.15f,0.15f,0.15f);
                _terrainEffect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                _terrainEffect.DirectionalLight0.Direction = new Vector3(1, 0.5f, 0);

                _terrainEffect.VertexColorEnabled = true;

                _terrainEffect.FogEnabled = true;
                _terrainEffect.FogColor = Color.DarkGray.ToVector3();
                _terrainEffect.FogStart = 120f;
                _terrainEffect.FogEnd = 160f;

                _terrainEffect.TextureEnabled = true;
            }

            //Sends Vertex Information to the graphics-card
            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionTextureColorNormal), GetVertexCount(), BufferUsage.WriteOnly);
                                    
            if(GetVertexCount() != 0) //Expensive Computation
                _vertexBuffer.SetData(_vertexPositions);
        }

        public void Draw(Matrix projectMatrix, Matrix viewMatrix, Matrix worldMatrix, Texture2D texture2D)
        {
            _terrainEffect.Projection = projectMatrix;
            _terrainEffect.View = viewMatrix;
            _terrainEffect.World = worldMatrix;

            _terrainEffect.Texture = texture2D;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            foreach (EffectPass pass in _terrainEffect.CurrentTechnique.Passes)
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

        private VertexPositionTextureColorNormal[] GenerateVertexPositions(Tile[,] tiles)
        {
            _primitiveCount = _width * _height * 2;
            
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            VertexPositionTextureColorNormal[] vertexPositions = new VertexPositionTextureColorNormal[GetVertexCount()];

            int vertexIndex = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {                    
                    var topLeft = new Vector3 (x-0.5f,  tiles[x,y].TopLeft / IncrementsPerHeightUnit, y-0.5f);
                    var topRight = new Vector3 (x+0.5f,  tiles[x,y].TopRight/ IncrementsPerHeightUnit, y-0.5f);
                    var bottomLeft = new Vector3 (x-0.5f, tiles[x,y].BottomLeft/ IncrementsPerHeightUnit, y+0.5f);
                    var bottomRight = new Vector3 (x+0.5f, tiles[x,y].BottomRight/ IncrementsPerHeightUnit, y+0.5f);

                    var colorTopLeft = Colors.ColorList[tiles[x, y].ColorTopLeft].Color;
                    var colorTopRight = Colors.ColorList[tiles[x, y].ColorTopRight].Color;
                    var colorBottomLeft = Colors.ColorList[tiles[x, y].ColorBottomLeft].Color;
                    var colorBottomRight = Colors.ColorList[tiles[x, y].ColorBottomRight].Color;
                            
                    Vector3 normal;
                    
                    //Allow tiletextures to be null so they wont be drawn.
                    if (tiles[x, y].TileTextureIdTriangleOne == null)
                        _primitiveCount--;
                    else
                    {
                        var texturePositionTexture0 = Textures.TextureList[(ushort) tiles[x,y].TileTextureIdTriangleOne].GetAdjustedTexturePosition();

                        if (tiles[x, y].IsAlternativeDiagonal)
                        {
                            normal = GenerateNormalDirection(bottomLeft, topLeft, bottomRight);
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture0.X,texturePositionTexture0.Y));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture0.X,texturePositionTexture0.W));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.Y));
                        }
                        else
                        {
                            normal = GenerateNormalDirection(topRight, bottomRight, bottomLeft);
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.W));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture0.Z,texturePositionTexture0.Y));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture0.X,texturePositionTexture0.Y));
                        }
                    }

                    if (tiles[x, y].TileTextureIdTriangleTwo == null)
                        _primitiveCount--;
                    else
                    {
                        var texturePositionTexture1 = Textures.TextureList[(ushort) tiles[x,y].TileTextureIdTriangleTwo].GetAdjustedTexturePosition();
                        
                        if (tiles[x,y].IsAlternativeDiagonal)
                        {                          
                            normal = GenerateNormalDirection(topLeft, topRight, bottomRight);
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture1.X,texturePositionTexture1.W));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.W));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomRight, normal, colorBottomRight, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.Y));
                        }
                        else
                        {     
                            normal = GenerateNormalDirection(bottomLeft, topLeft, topRight);
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(bottomLeft, normal, colorBottomLeft, new Vector2(texturePositionTexture1.X,texturePositionTexture1.Y));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topLeft, normal, colorTopLeft, new Vector2(texturePositionTexture1.X,texturePositionTexture1.W));
                            vertexPositions[vertexIndex++] = new VertexPositionTextureColorNormal(topRight, normal, colorTopRight, new Vector2(texturePositionTexture1.Z,texturePositionTexture1.W));
                        }
                    }
                }
            }
            
            Array.Resize(ref vertexPositions, GetVertexCount());

            return vertexPositions;
        }

        public static byte[,] GenerateHeightMapFromTileMap(Tile[,] tilemap)
        {
            byte[,] heightmap = new byte[tilemap.GetLength(0)+1,tilemap.GetLength(1)+1];
            
            for (var x = 0; x < heightmap.GetLength(0); x++)
            {
                for (var y = 0; y < heightmap.GetLength(1); y++)
                {
                    var right = x == tilemap.GetLength(0);
                    var bottom = y == tilemap.GetLength(1);

                    if (right || bottom)
                    {
                        if (right && !bottom)
                        {
                            heightmap[x, y] = tilemap[x-1, y].TopRight;
                        }
                        if (!right && bottom)
                        {
                            heightmap[x, y] = tilemap[x, y-1].BottomLeft;
                        }
                        if (right && bottom)
                        {
                            heightmap[x, y] = tilemap[x-1, y-1].BottomRight;
                        }
                    }
                    else
                    {
                        heightmap[x, y] = tilemap[x, y].TopLeft;
                    }
                }
            }

            return heightmap;
        }

        public static Tile[,] GenerateTileMap(byte[,] heightmap)
        {
            int width = heightmap.GetLength(0)-1;
            int height = heightmap.GetLength(1)-1;
            
            Tile[,] tempTileMap = new Tile[width,height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var topLeft = heightmap[x,y];
                    var topRight = heightmap[x+1,y];
                    var bottomLeft = heightmap[x,y+1];
                    var bottomRight = heightmap[x+1,y+1];
                    
                    var isAlternativeDiagonal = IsAlternativeDiagonal(
                        new Vector3(x, topLeft, y),
                        new Vector3(x+1, topRight, y),
                        new Vector3(x+1, bottomRight, y+1),
                        new Vector3(x, bottomLeft, y+1)
                        );
                    
                    ushort? textureId = Textures.GetDefaultTextureId();

                    if (topLeft > 31)
                        textureId = null;

                    var colortl = Colors.GetDefaultColorId();
                    var colortr = colortl;
                    var colorbl = colortl;
                    var colorbr = colortl;
                    
                    if (topLeft < 2)
                        colortl = 6;
                    else if (topLeft < 8)
                        colortl = 7;
                    else
                        colortl = 8;
                    
                    if (topRight < 2)
                        colortr = 6;
                    else if (topRight < 8)
                        colortr = 7;
                    else
                        colortr = 8;
                    
                    if (bottomLeft < 2)
                        colorbl = 6;
                    else if (bottomLeft < 8)
                        colorbl = 7;
                    else
                        colorbl = 8;
                    
                    if (bottomRight < 2)
                        colorbr = 6;
                    else if (bottomRight < 8)
                        colorbr = 7;
                    else
                        colorbr = 8;
                    
                    tempTileMap[x,y] = new Tile()
                    {
                        TopLeft =  topLeft,
                        TopRight = topRight,
                        BottomLeft = bottomLeft,
                        BottomRight = bottomRight,
                        IsAlternativeDiagonal = isAlternativeDiagonal,
                        TileTextureIdTriangleOne = textureId,
                        TileTextureIdTriangleTwo = textureId,
                        ColorBottomLeft = colorbl,
                        ColorTopLeft = colortl,
                        ColorBottomRight = colorbr,
                        ColorTopRight = colortr,
                    };
                }
            }

            return tempTileMap;
        }

        public static Tile[,] GenerateTileMap(int chunkSize, byte floor)
        {
            var heightmap = new byte[chunkSize+1,chunkSize+1];

            for (var x = 0; x < heightmap.GetLength(0); x++)
            {
                for (var y = 0; y < heightmap.GetLength(1); y++)
                {
                    heightmap[x, y] = (byte) (floor * DefaultDistanceBetweenFloors);
                }
            }
            
            return GenerateTileMap(heightmap);
        }

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
    }
}
