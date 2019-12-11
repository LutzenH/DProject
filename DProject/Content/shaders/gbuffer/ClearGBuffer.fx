struct VertexShaderInput
{
    float3 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = float4(input.Position, 1);

    return output;
}

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
    float4 Light : COLOR3;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;

    output.Color = 0.0f;
    output.Color.a = 0.0f;

    output.Normal.rgb = 0.5f;
    output.Normal.a = 0.0f;

    output.Depth = 1.0f;

    output.Light = 1.0f;

    return output;
}

technique ClearGBuffer
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
