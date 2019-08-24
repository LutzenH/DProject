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
float4x4 Projection;

texture heightmapTexture;
sampler2D heightmapSampler = sampler_state
{
	Texture = <heightmapTexture>;
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
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float2 xy = ClipMapOffset + mul(input.Position, World).xy * ClipMapScale;
	float2 uv = (xy + 0.5) / TextureDimension;
	
	float4 heightSample = tex2Dlod(heightmapSampler, float4(uv, 0, 0));
	
	float4 elevatedPos = float4(input.Position.x, heightSample.r, input.Position.z, input.Position.w);
	
    float4 worldPosition = mul(elevatedPos, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.UV = uv;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return float4(1,1,1,1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
