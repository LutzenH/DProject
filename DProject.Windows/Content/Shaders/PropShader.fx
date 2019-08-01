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
float4x4 ReflectionView;
float4x4 Projection;

float WaterHeight;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float3 Normal : NORMAL0;
    float2 VertexHeight : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
	output.Color = input.Color;
	output.Normal = input.Normal;
	output.VertexHeight = float2(worldPosition.y, 0.0f);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

VertexShaderOutput ReflectionVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, ReflectionView);
    output.Position = mul(viewPosition, Projection);
    
	output.Color = input.Color;
	output.Normal = input.Normal;
	output.VertexHeight = float2(worldPosition.y, 0.0f);

	return output;
}

float4 ReflectionPS(VertexShaderOutput input) : COLOR
{
    clip(input.VertexHeight.x - WaterHeight - WaterHeight);
	return MainPS(input);
}

float4 RefractionPS(VertexShaderOutput input) : COLOR
{
    clip(WaterHeight - input.VertexHeight.x + WaterHeight);
	return MainPS(input);
}

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