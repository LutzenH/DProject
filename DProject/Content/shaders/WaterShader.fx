#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

float NearClip;
float FarClip;
float MaxWaterDepth;

float3 WaterColor;
float3 DeepWaterColor;

float MinimumFoamDistance;
float MaximumFoamDistance;

texture reflectionTexture;
texture refractionTexture;
texture depthTexture;
texture dudvTexture;

float RelativeGameTime;
float Tiling;
float DistortionIntensity;
float FresnelIntensity;
float WaterSpeed;

sampler2D reflectionSampler = sampler_state
{
	Texture = <reflectionTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

sampler2D refractionSampler = sampler_state
{
	Texture = <refractionTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

sampler2D depthSampler = sampler_state
{
	Texture = <depthTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

sampler2D dudvSampler = sampler_state
{
	Texture = <dudvTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
    float3 toCameraVector : TEXCOORD2;
    float4 ClipSpace : TEXCOORD3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate * Tiling;

    output.Normal = input.Normal;
    
    output.ClipSpace = output.Position;

    output.toCameraVector = CameraPosition - worldPosition.xyz;
    
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 viewVector = normalize(input.toCameraVector);
    float refractiveFactor = 1-dot(viewVector, input.Normal);
    refractiveFactor = pow(refractiveFactor, FresnelIntensity);

    float2 ndc = (input.ClipSpace.xy/input.ClipSpace.w) / 2 + 0.5f;
    float2 reflectionndc = float2(ndc.x, ndc.y);
    
    float2 regularndc = float2(ndc.x, -ndc.y);
    float2 refractionndc = regularndc;
        
    float2 distortion = (tex2D(dudvSampler, float2(input.TextureCoordinate.x + RelativeGameTime * WaterSpeed,  input.TextureCoordinate.y)).rg * 2.0 - 1.0) * DistortionIntensity;
    
    reflectionndc += distortion;
    reflectionndc = clamp(reflectionndc, 0.001, 0.999);
    
    refractionndc += distortion;
    refractionndc.x = clamp(refractionndc.x, 0.001, 0.999);
    refractionndc.y = clamp(refractionndc.y, -0.999, -0.001);
    
    float4 reflectionColor = tex2D(reflectionSampler, reflectionndc);
    float4 refractionColor = tex2D(refractionSampler, refractionndc);

    refractionColor = lerp(float4(DeepWaterColor, 1), refractionColor, 0.5);

    float distanceToWater = (input.ClipSpace.w + NearClip) / FarClip;

    float4 depthbufferColor = tex2D(depthSampler, regularndc);

    float depth = depthbufferColor.a - distanceToWater;
    float waterDepthFactor = clamp(depth / (MaxWaterDepth / FarClip), 0.0, 1.0);
    
    refractionColor = lerp(refractionColor, float4(WaterColor, 1), waterDepthFactor/1.1);
        
    float4 color = lerp(refractionColor, reflectionColor, refractiveFactor);

    float foamDistance = lerp(MinimumFoamDistance, MaximumFoamDistance, depthbufferColor.xyz);

    float waterEdgeFactor = smoothstep(0.0, 1.0, waterDepthFactor / foamDistance);

    color = lerp(float4(1,1,1,1), color, waterEdgeFactor);

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
