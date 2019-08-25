using DProject.Type.Rendering.Shaders.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering.Shaders
{
    public class WaterEffect : AbstractEffect, IReflective, INeedClipPlanes
    {
        public WaterEffect(Effect cloneSource) : base(cloneSource) { }

        public Vector3 CameraPosition
        {
            get => Parameters["CameraPosition"].GetValueVector3();
            set => Parameters["CameraPosition"].SetValue(value);
        }

        public Texture2D ReflectionBuffer
        {
            get => Parameters["reflectionTexture"].GetValueTexture2D();
            set => Parameters["reflectionTexture"].SetValue(value);
        }

        public Texture2D RefractionBuffer
        {
            get => Parameters["refractionTexture"].GetValueTexture2D();
            set => Parameters["refractionTexture"].SetValue(value);
        }

        public Texture2D DepthBuffer
        {
            get => Parameters["depthTexture"].GetValueTexture2D();
            set => Parameters["depthTexture"].SetValue(value);
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
        
        public float MaxWaterDepth
        {
            get => Parameters["MaxWaterDepth"].GetValueSingle();
            set => Parameters["MaxWaterDepth"].SetValue(value);
        }
        
        public Texture2D DuDvTexture
        {
            get => Parameters["dudvTexture"].GetValueTexture2D();
            set => Parameters["dudvTexture"].SetValue(value);
        }
        
        public float RelativeGameTime
        {
            get => Parameters["RelativeGameTime"].GetValueSingle();
            set => Parameters["RelativeGameTime"].SetValue(value);
        }

        public float DuDvTiling
        {
            get => Parameters["Tiling"].GetValueSingle();
            set => Parameters["Tiling"].SetValue(value);
        }
        
        public float DistortionIntensity
        {
            get => Parameters["DistortionIntensity"].GetValueSingle();
            set => Parameters["DistortionIntensity"].SetValue(value);
        }
        
        public float FresnelIntensity
        {
            get => Parameters["FresnelIntensity"].GetValueSingle();
            set => Parameters["FresnelIntensity"].SetValue(value);
        }
        
        public float WaterSpeed
        {
            get => Parameters["WaterSpeed"].GetValueSingle();
            set => Parameters["WaterSpeed"].SetValue(value);
        }

        public Vector3 WaterColor
        {
            get => Parameters["WaterColor"].GetValueVector3();
            set => Parameters["WaterColor"].SetValue(value);
        }
        
        public Vector3 DeepWaterColor
        {
            get => Parameters["DeepWaterColor"].GetValueVector3();
            set => Parameters["DeepWaterColor"].SetValue(value);
        }

        public float MinimumFoamDistance
        {
            get => Parameters["MinimumFoamDistance"].GetValueSingle();
            set => Parameters["MinimumFoamDistance"].SetValue(value);
        }
        
        public float MaximumFoamDistance
        {
            get => Parameters["MaximumFoamDistance"].GetValueSingle();
            set => Parameters["MaximumFoamDistance"].SetValue(value);
        }
    }
}
 
