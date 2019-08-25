using DProject.Game.Component.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Terrain
{
    public class ClipMapTerrainMeshLoaderSystem : EntityProcessingSystem
    {
        public const int MeshSize = 48;

        private readonly GraphicsDevice _graphicsDevice;

        private ComponentMapper<ClipMapTerrainComponent> _clipMapTerrainMapper;
        private ComponentMapper<LoadedClipMapTerrainComponent> _loadedClipMapTerrainMapper;

        public ClipMapTerrainMeshLoaderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(ClipMapTerrainComponent)))
        {
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _clipMapTerrainMapper = mapperService.GetMapper<ClipMapTerrainComponent>();
            _loadedClipMapTerrainMapper = mapperService.GetMapper<LoadedClipMapTerrainComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var (tileVertexBuffer, tileIndexBuffer) = GenerateTileMeshBuffers(MeshSize, _graphicsDevice);
            var (crossVertexBuffer, crossIndexBuffer) = GenerateCrossMeshBuffers(MeshSize, _graphicsDevice);
            var (fillerVertexBuffer, fillerIndexBuffer) = GenerateFillerMeshBuffers(MeshSize, _graphicsDevice);
            var (seamVertexBuffer, seamIndexBuffer) = GenerateSeamMeshBuffers(MeshSize, _graphicsDevice);
            var (trimVertexBuffer, trimIndexBuffer) = GenerateTrimMeshBuffers(MeshSize, _graphicsDevice);
            
            var loadedClipMapTerrainComponent = new LoadedClipMapTerrainComponent
            {
                TileVertexBuffer = tileVertexBuffer,
                TileIndexBuffer = tileIndexBuffer,
                
                CrossVertexBuffer = crossVertexBuffer,
                CrossIndexBuffer = crossIndexBuffer,
                
                FillerVertexBuffer = fillerVertexBuffer,
                FillerIndexBuffer = fillerIndexBuffer,
                
                SeamVertexBuffer = seamVertexBuffer,
                SeamIndexBuffer = seamIndexBuffer,
                
                TrimVertexBuffer = trimVertexBuffer,
                TrimIndexBuffer = trimIndexBuffer
            };
            
            _clipMapTerrainMapper.Delete(entityId);
            _loadedClipMapTerrainMapper.Put(entityId, loadedClipMapTerrainComponent);
        }

        #region Tile Mesh

        private static (VertexBuffer, IndexBuffer) GenerateTileMeshBuffers(int size, GraphicsDevice graphicsDevice)
        {
            var (vertexPositions, indices) = GenerateTileMesh(size);

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            
            vertexBuffer.SetData(vertexPositions);
            indexBuffer.SetData(indices);

            return (vertexBuffer, indexBuffer);
        }

        private static (VertexPosition[], int[]) GenerateTileMesh(int size)
        {
            return (GenerateTileVertexPositions(size + 1), GenerateTileIndices(size + 1));
        }

        private static VertexPosition[] GenerateTileVertexPositions(int size)
        {
            var vertices = new VertexPosition[size * size];

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                    vertices[x + y * size].Position = new Vector3(x, 0, y);
            }

            return vertices;
        }
        
        private static int[] GenerateTileIndices(int size)
        {
            var indices = new int[(size - 1) * (size - 1) * 6];
            var n = 0;
            
            for (var y = 0; y < size - 1; y++)
            {
                for (var x = 0; x < size - 1; x++)
                {
                    var topLeft = x + y*size;
                    var topRight = (x + 1) + y*size;
                    var bottomLeft = x + (y + 1) * size;
                    var bottomRight = (x + 1) + (y + 1) * size;
 
                    indices[n++] = topLeft;
                    indices[n++] = bottomRight;
                    indices[n++] = bottomLeft;
                    
                    indices[n++] = topLeft;
                    indices[n++] = topRight;
                    indices[n++] = bottomRight;
                }
            }

            return indices;
        }

        #endregion

        #region Cross Mesh

        private static (VertexBuffer, IndexBuffer) GenerateCrossMeshBuffers(int size, GraphicsDevice graphicsDevice)
        {
            var (vertexPositions, indices) = GenerateCrossMesh(size);

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            
            vertexBuffer.SetData(vertexPositions);
            indexBuffer.SetData(indices);

            return (vertexBuffer, indexBuffer);
        }

        private static (VertexPosition[], int[]) GenerateCrossMesh(int size)
        {
            var (vertexPositions, startOfVertical) = GenerateCrossVertexPositions(size);
            return (vertexPositions, GenerateCrossIndices(size, startOfVertical));
        }

        private static (VertexPosition[], int) GenerateCrossVertexPositions(int size)
        {
            var vertices = new VertexPosition[(size + 1) * 8];
            var n = 0;
            
            // Horizontal vertices
            for(var i = 0; i < (size + 1) * 2; i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(i - size, 0, 0));
                vertices[n++] = new VertexPosition(new Vector3(i - size, 0, 1));
            }

            var startOfVertical = n;
            
            // Vertical vertices
            for(var i = 0; i < (size + 1) * 2; i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(0, 0, i - size));
                vertices[n++] = new VertexPosition(new Vector3(1, 0, i - size));
            }

            return (vertices, startOfVertical);
        }
        
        private static int[] GenerateCrossIndices(int size, int startOfVertical)
        {
            var indices = new int[size * 24 + 6];
            var n = 0;
            
            // Horizontal indices
            for(var i = 0; i < size * 2 + 1; i++ ) {
                var bl = i * 2 + 0;
                var br = i * 2 + 1;
                var tl = i * 2 + 2;
                var tr = i * 2 + 3;
                
                indices[ n++ ] = br;
                indices[ n++ ] = bl;
                indices[ n++ ] = tr;
                indices[ n++ ] = bl;
                indices[ n++ ] = tl;
                indices[ n++ ] = tr;
            }
            
            // Vertical indices
            for(var i = 0; i < size * 2 + 1; i++ ) {
                if( i == size )
                    continue;
                
                var bl = i * 2 + 0;
                var br = i * 2 + 1;
                var tl = i * 2 + 2;
                var tr = i * 2 + 3;
                
                indices[n++] = startOfVertical + br;
                indices[n++] = startOfVertical + tr;
                indices[n++] = startOfVertical + bl;
                indices[n++] = startOfVertical + bl; 
                indices[n++] = startOfVertical + tr;
                indices[n++] = startOfVertical + tl;
            }
            
            return indices;
        }

        #endregion

        #region Filler Mesh

        private static (VertexBuffer, IndexBuffer) GenerateFillerMeshBuffers(int size, GraphicsDevice graphicsDevice)
        {
            var (vertexPositions, indices) = GenerateFillerMesh(size);

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            
            vertexBuffer.SetData(vertexPositions);
            indexBuffer.SetData(indices);

            return (vertexBuffer, indexBuffer);
        }

        private static (VertexPosition[], int[]) GenerateFillerMesh(int size)
        {
            return (GenerateFillerVertexPositions(size), GenerateFillerIndices(size));
        }

        private static VertexPosition[] GenerateFillerVertexPositions(int size)
        {
            var vertices = new VertexPosition[(size+1) * 8];
            var n = 0;
            var offset = size;

            for(var i = 0; i < (size+1); i++ )
            {
                vertices[n++] = new VertexPosition(new Vector3(offset + i + 1, 0, 0));
                vertices[n++] = new VertexPosition(new Vector3(offset + i + 1, 0, 1));
            }

            for(var i = 0; i < (size+1); i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(1, 0, offset + i + 1));
                vertices[n++] = new VertexPosition(new Vector3(0, 0, offset + i + 1));
            }

            for(var i = 0; i < (size+1); i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(-(offset + i), 0, 1));
                vertices[n++] = new VertexPosition(new Vector3(-(offset + i), 0, 0));
            }

            for(var i = 0; i < (size+1); i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(0, 0, -(offset + i)));
                vertices[n++] = new VertexPosition(new Vector3(1, 0, -(offset + i)));
            }

            return vertices;
        }
        
        private static int[] GenerateFillerIndices(int size)
        {
            var indices = new int[size * 24];
            var n = 0;
            
            for(var i = 0; i < size * 4; i++ ) {
                // the arms shouldn't be connected to each other
                var arm = i / size;

                var bl = ( arm + i ) * 2 + 0;
                var br = ( arm + i ) * 2 + 1;
                var tl = ( arm + i ) * 2 + 2;
                var tr = ( arm + i ) * 2 + 3;

                if( arm % 2 == 0 ) {
                    indices[ n++ ] = br;
                    indices[ n++ ] = bl;
                    indices[ n++ ] = tr;
                    indices[ n++ ] = bl;
                    indices[ n++ ] = tl;
                    indices[ n++ ] = tr;
                }
                else {
                    indices[ n++ ] = br;
                    indices[ n++ ] = bl;
                    indices[ n++ ] = tl;
                    indices[ n++ ] = br;
                    indices[ n++ ] = tl;
                    indices[ n++ ] = tr;
                }
            }

            return indices;
        }

        #endregion

        #region Seam Mesh

        private static (VertexBuffer, IndexBuffer) GenerateSeamMeshBuffers(int size, GraphicsDevice graphicsDevice)
        {
            var (vertexPositions, indices) = GenerateSeamMesh(size);

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            
            vertexBuffer.SetData(vertexPositions);
            indexBuffer.SetData(indices);

            return (vertexBuffer, indexBuffer);
        }

        private static (VertexPosition[], int[]) GenerateSeamMesh(int size)
        {
            return (GenerateSeamVertexPositions(size), GenerateSeamIndices(size));
        }

        private static VertexPosition[] GenerateSeamVertexPositions(int size)
        {
            var clipMapResolution = size * 4 + 1;
            var clipMapVertResolution = clipMapResolution + 1;
            
            var vertices = new VertexPosition[clipMapVertResolution * 4];

            for(var i = 0; i < clipMapVertResolution; i++) {
                vertices[clipMapVertResolution * 0 + i] = new VertexPosition(new Vector3(i, 0, 0));
                vertices[clipMapVertResolution * 1 + i] = new VertexPosition(new Vector3(clipMapVertResolution, 0, i));
                vertices[clipMapVertResolution * 2 + i] = new VertexPosition(new Vector3(clipMapVertResolution - 1, 0, clipMapVertResolution));
                vertices[clipMapVertResolution * 3 + i] = new VertexPosition(new Vector3(0, 0, clipMapVertResolution - 1));
            }

            return vertices;
        }
        
        private static int[] GenerateSeamIndices(int size)
        {
            var clipMapResolution = size * 4 + 1;
            var clipMapVertResolution = clipMapResolution + 1;
            
            var indices = new int[clipMapVertResolution * 6];
            var n = 0;
            
            for(var i = 0; i < clipMapVertResolution * 4; i += 2 ) {
                indices[n++] = i + 1;
                indices[n++] = i;
                indices[n++] = i + 2;
            }

            indices[indices.Length - 1 ] = 0;
            
            return indices;
        }

        #endregion

        #region Trim Mesh

        private static (VertexBuffer, IndexBuffer) GenerateTrimMeshBuffers(int size, GraphicsDevice graphicsDevice)
        {
            var (vertexPositions, indices) = GenerateTrimMesh(size);

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            
            vertexBuffer.SetData(vertexPositions);
            indexBuffer.SetData(indices);

            return (vertexBuffer, indexBuffer);
        }

        private static (VertexPosition[], int[]) GenerateTrimMesh(int size)
        {
            var (vertexPositions, startOfHorizontal) = GenerateTrimVertexPositions(size);
            return (vertexPositions, GenerateTrimIndices(size, startOfHorizontal));
        }

        private static (VertexPosition[], int) GenerateTrimVertexPositions(int size)
        {
            var clipMapResolution = size * 4 + 1;
            var clipMapVertResolution = clipMapResolution + 1;
            
            var subPosition = new Vector3(
                0.5f * (clipMapVertResolution + 1),
                0,
                0.5f * (clipMapVertResolution + 1));
            
            var vertices = new VertexPosition[(clipMapVertResolution * 2 + 1) * 2];
            var n = 0;

            // vertical part of L
            for(var i = 0; i < clipMapVertResolution + 1; i++) {
                vertices[n++] = new VertexPosition(new Vector3(0, 0, clipMapVertResolution - i) - subPosition);
                vertices[n++] = new VertexPosition(new Vector3(1, 0, clipMapVertResolution - i) - subPosition);
            }
            
            var startOfHorizontal = n;
            
            // horizontal part of L
            for(var i = 0; i < clipMapVertResolution; i++ ) {
                vertices[n++] = new VertexPosition(new Vector3(i + 1, 0, 0) - subPosition);
                vertices[n++] = new VertexPosition(new Vector3(i + 1, 0, 1) - subPosition);
            }

            return (vertices, startOfHorizontal);
        }
        
        private static int[] GenerateTrimIndices(int size, int startOfHorizontal)
        {
            var clipMapResolution = size * 4 + 1;
            var clipMapVertResolution = clipMapResolution + 1;
            
            var indices = new int[(clipMapVertResolution * 2 - 1) * 6];
            var n = 0;
            
            for(var i = 0; i < clipMapVertResolution; i++ ) {
                indices[ n++ ] = ( i + 0 ) * 2 + 1;
                indices[ n++ ] = ( i + 0 ) * 2 + 0;
                indices[ n++ ] = ( i + 1 ) * 2 + 0;
                
                indices[ n++ ] = ( i + 1 ) * 2 + 1;
                indices[ n++ ] = ( i + 0 ) * 2 + 1;
                indices[ n++ ] = ( i + 1 ) * 2 + 0;
            }
            
            for(var i = 0; i < clipMapVertResolution - 1; i++ ) {
                indices[ n++ ] = startOfHorizontal + ( i + 0 ) * 2 + 1;
                indices[ n++ ] = startOfHorizontal + ( i + 0 ) * 2 + 0;
                indices[ n++ ] = startOfHorizontal + ( i + 1 ) * 2 + 0;
                
                indices[ n++ ] = startOfHorizontal + ( i + 1 ) * 2 + 1;
                indices[ n++ ] = startOfHorizontal + ( i + 0 ) * 2 + 1;
                indices[ n++ ] = startOfHorizontal + ( i + 1 ) * 2 + 0;
            }

            return indices;
        }

        #endregion
    }
}
