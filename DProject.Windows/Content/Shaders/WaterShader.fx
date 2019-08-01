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

texture reflectionTexture;
texture refractionTexture;

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

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 toCameraVector : NORMAL1;
    float4 ClipSpace : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;

    output.Normal = input.Normal;
    
    output.ClipSpace = output.Position;

    output.toCameraVector = CameraPosition - worldPosition.xyz;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 viewVector = normalize(input.toCameraVector);
    float refractiveFactor = 1-dot(viewVector, input.Normal);

    float2 ndc = (input.ClipSpace.xy/input.ClipSpace.w) / 2 + 0.5f;
    
    float4 reflectionColor = tex2D(reflectionSampler, ndc);
    float4 refractionColor = tex2D(refractionSampler, float2(ndc.x, -ndc.y));

	return lerp(refractionColor, reflectionColor, refractiveFactor);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
