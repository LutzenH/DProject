using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class ClipMapTerrainEffect : AbstractEffect, INeedClipPlanes, IReflected
    {
        public ClipMapTerrainEffect(Effect cloneSource) : base(cloneSource) { }
        
        public Texture2D Diffuse
        {
            get => Parameters["diffuseTexture"].GetValueTexture2D();
            set => Parameters["diffuseTexture"].SetValue(value);
        }
        
        public Texture2D Heightmap
        {
            get => Parameters["heightmapTexture"].GetValueTexture2D();
            set => Parameters["heightmapTexture"].SetValue(value);
        }
        
        public Texture2D NormalMap
        {
            get => Parameters["normalTexture"].GetValueTexture2D();
            set => Parameters["normalTexture"].SetValue(value);
        }
        
        public Vector2 TextureDimension
        {
            get => Parameters["TextureDimension"].GetValueVector2();
            set => Parameters["TextureDimension"].SetValue(value);
        }
        
        public Vector2 ClipMapOffset
        {
            get => Parameters["ClipMapOffset"].GetValueVector2();
            set => Parameters["ClipMapOffset"].SetValue(value);
        }
        
        public float ClipMapScale
        {
            get => Parameters["ClipMapScale"].GetValueSingle();
            set => Parameters["ClipMapScale"].SetValue(value);
        }

        public float NearClipPlane
        {
            get => Parameters["NearClip"].GetValueSingle();
            set => Parameters["NearClip"].SetValue(value);
        }
        
        public float FarClipPlane
        {
            get => Parameters["FarClip"].GetValueSingle();
            set => Parameters["FarClip"].SetValue(value);
        }

        public Matrix ReflectionView
        {
            get => Parameters["ReflectionView"].GetValueMatrix();
            set => Parameters["ReflectionView"].SetValue(value);
        }

        public float WaterHeight
        {
            get => Parameters["WaterHeight"].GetValueSingle();
            set => Parameters["WaterHeight"].SetValue(value);
        }
    }
}
