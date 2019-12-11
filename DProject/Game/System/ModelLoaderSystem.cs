using System.IO;
using DProject.Game.Component;
using DProject.Type.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System
{
    public class ModelLoaderSystem : EntityUpdateSystem
    {
        private readonly ContentManager _contentManager;
        
        private ComponentMapper<ModelComponent> _modelMapper;
        private ComponentMapper<LoadedModelComponent> _loadedModelMapper;

        public ModelLoaderSystem(ContentManager contentManager) : base(Aspect.All(typeof(ModelComponent)))
        {
            _contentManager = contentManager;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _modelMapper = mapperService.GetMapper<ModelComponent>();
            _loadedModelMapper = mapperService.GetMapper<LoadedModelComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var modelComponent = _modelMapper.Get(entity);
                
                var loadedModelComponent = new LoadedModelComponent()
                {
                    Model = _contentManager.Load<Model>(modelComponent.ModelPath)
                };
                
                _loadedModelMapper.Put(entity, loadedModelComponent);
                _modelMapper.Delete(entity);
            }
        }
        
        #region Format Converter
        public static (VertexBuffer, IndexBuffer, int) ConvertDProjectModelFormatToModel(string pathToFile, GraphicsDevice graphicsDevice)
        {
            var fileStream = new FileStream(pathToFile, FileMode.Open);
            var binaryReader = new BinaryReader(fileStream);

            var objectName = binaryReader.ReadString();
            var vertexCount = binaryReader.ReadUInt32();
            var indexCount = binaryReader.ReadUInt32();
            var indexIsThirtyTwoBits = binaryReader.ReadBoolean();
            var vertexDeclarationMask = binaryReader.ReadByte();
            
            var useUvCoords = (vertexDeclarationMask & (1 << 0)) != 0;
            var useVertexColor = (vertexDeclarationMask & (1 << 1)) != 0;
            var useSpecularPower = (vertexDeclarationMask & (1 << 2)) != 0;
            var useSpecularIntensity = (vertexDeclarationMask & (1 << 3)) != 0;
            var useEmission = (vertexDeclarationMask & (1 << 4)) != 0;

            var vertices = new VertexPositionNormalTextureColorLight[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = new VertexPositionNormalTextureColorLight();
                
                var x = binaryReader.ReadSingle();
                var y = binaryReader.ReadSingle();
                var z = binaryReader.ReadSingle();
                vertex.Position = new Vector3(x, y, z);
                
                var nx = binaryReader.ReadSingle();
                var ny = binaryReader.ReadSingle();
                var nz = binaryReader.ReadSingle();
                vertex.Normal = new Vector3(nx, ny, nz);

                vertex.TextureCoordinate = Vector2.Zero;
                vertex.Color = Color.Black;
                vertex.LightingInfo = Color.Black;
                
                if (useUvCoords)
                {
                    var uv_u = binaryReader.ReadSingle();
                    var uv_v = binaryReader.ReadSingle();
                    
                    vertex.TextureCoordinate = new Vector2(uv_u, uv_v);
                }

                if (useVertexColor)
                {
                    var red = binaryReader.ReadByte();
                    var green = binaryReader.ReadByte();
                    var blue = binaryReader.ReadByte();
                    
                    vertex.Color = new Color(red, green, blue);
                }

                byte specular_power, specular_intensity, emission;
                specular_power = specular_intensity = emission = 0;
                
                if (useSpecularPower)
                    specular_power = binaryReader.ReadByte();

                if (useSpecularIntensity)
                    specular_intensity = binaryReader.ReadByte();

                if (useEmission)
                    emission = binaryReader.ReadByte();
                
                vertex.LightingInfo = new Color(specular_power, specular_intensity, emission);

                vertices[i] = vertex;
            }

            var vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTextureColorLight), (int) vertexCount, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            IndexBuffer indexBuffer;
            
            if (indexIsThirtyTwoBits)
            {
                var indicies = new uint[indexCount];

                for (var i = 0; i < indexCount; i++)
                    indicies[i] = binaryReader.ReadUInt32();
                
                indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, (int) indexCount, BufferUsage.WriteOnly);
                indexBuffer.SetData(indicies);
            }
            else
            {
                var indicies = new ushort[indexCount];

                for (var i = 0; i < indexCount; i++)
                    indicies[i] = binaryReader.ReadUInt16();
                
                indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, (int) indexCount, BufferUsage.WriteOnly);
                indexBuffer.SetData(indicies);
            }
            
            return (vertexBuffer, indexBuffer, (int) indexCount/3);
        }
        #endregion
    }
}
