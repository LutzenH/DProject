float4x4 World;
float4x4 View;
float4x4 Projection;

float SpecularIntensity;
float SpecularPower;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float2 Depth : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);

    output.Color = input.Color;

    output.TexCoord = input.TexCoord;

    output.Normal = mul(input.Normal, World);

    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    return output;
}

struct PixelShaderOutput
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;

    output.Color = input.Color;
    output.Color.a = SpecularIntensity;

    output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
    output.Normal.a = SpecularPower;

    output.Depth = input.Depth.x / input.Depth.y;

    return output;
}

technique RenderGBuffer
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
