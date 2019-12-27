float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 Depth : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(float4 Position : SV_POSITION)
{
    VertexShaderOutput output;
    
    float4 worldPosition = mul(Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Depth = float2(output.Position.z, output.Position.w);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return 1 - (input.Depth.x / input.Depth.y);
}

technique ShadowMap
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
