#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

float4x4 World;
float4x4 View;
float4x4 ReflectionView;
float4x4 Projection;

float WaterHeight;

float NearClip;
float FarClip;

texture diffuseTexture;
sampler2D diffuseSampler = sampler_state
{
	Texture = <diffuseTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

texture heightmapTexture;
sampler2D heightmapSampler = sampler_state
{
	Texture = <heightmapTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

texture normalTexture;
sampler2D normalSampler = sampler_state
{
	Texture = <normalTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

float2 TextureDimension;
float2 ClipMapOffset;
float ClipMapScale;

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
    float2 HeightSample : TEXCOORD1;
    float4 ViewPositionVS : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float2 xy = ClipMapOffset + mul(input.Position, World).xz * ClipMapScale;
	float2 uv = (xy + 0.5) / TextureDimension;
	
	float2 heightSample = tex2Dlod(heightmapSampler, float4(uv, 0, 0)).rg;
	float ScaledHeightSample = 512 * heightSample.r + heightSample.g * 2;
	
	float4 elevatedPos = float4(input.Position.x, ScaledHeightSample, input.Position.z, input.Position.w);
	
    float4 worldPosition = mul(elevatedPos, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.UV = uv;
    output.HeightSample = heightSample;
    output.ViewPositionVS = output.Position;

	return output;
}

VertexShaderOutput ReflectionVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float2 xy = ClipMapOffset + mul(input.Position, World).xz * ClipMapScale;
	float2 uv = (xy + 0.5) / TextureDimension;
	
	float2 heightSample = tex2Dlod(heightmapSampler, float4(uv, 0, 0)).rg;
	float ScaledHeightSample = 512 * heightSample.r + heightSample.g * 2;
	
	float4 elevatedPos = float4(input.Position.x, ScaledHeightSample, input.Position.z, input.Position.w);
	
    float4 worldPosition = mul(elevatedPos, World);
    float4 viewPosition = mul(worldPosition, ReflectionView);
    
    output.Position = mul(viewPosition, Projection);
    output.UV = uv;
    output.HeightSample = heightSample;
    output.ViewPositionVS = output.Position;

	return output;
}

float4 DepthPS(VertexShaderOutput input) : COLOR
{   
    float depth = (input.ViewPositionVS.w + NearClip) / FarClip;
    
    float3 normal = tex2D(normalSampler, input.UV).rgb;
    
	return float4(normal.xyz, depth);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 normal = tex2D(normalSampler, input.UV).rgb;
    
    float4 color = tex2D(diffuseSampler, input.UV);
    
    float normalIntensity = lerp(normal.y, normal.z, 0.5);
    color = mul(color, normalIntensity);
    
	return color;
}

float4 ReflectionPS(VertexShaderOutput input) : COLOR
{
    float height = 512 * input.HeightSample.x + input.HeightSample.y * 2;

    clip(height - WaterHeight);
	return MainPS(input);
}

float4 RefractionPS(VertexShaderOutput input) : COLOR
{
    float height = 512 * input.HeightSample.x + input.HeightSample.y * 2;

    clip(WaterHeight - height);
	return MainPS(input);
}

technique Depth
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL DepthPS();
	}
};

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

technique Reflection
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL ReflectionVS();
		PixelShader = compile PS_SHADERMODEL ReflectionPS();
	}
}

technique Refraction
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL RefractionPS();
    }
}
