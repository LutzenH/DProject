float4x4 World;
float4x4 View;
float4x4 Projection;

float NearClip;
float FarClip;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float4 ViewPositionVS : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);	
    output.Normal = mul(input.Normal, View);
    output.ViewPositionVS = output.Position;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float depth = (input.ViewPositionVS.w + NearClip) / FarClip;
    input.Normal = normalize(input.Normal);
    
    return float4(input.Normal.xyz, depth);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile vs_3_0 MainVS();
		PixelShader = compile ps_3_0 MainPS();
	}
};
