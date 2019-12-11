texture ColorMap;
sampler ColorSampler = sampler_state
{
    Texture = (ColorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture LightMap;
sampler LightSampler = sampler_state
{
    Texture = (LightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture LightInfoMap;
sampler LightInfoSampler = sampler_state
{
    Texture = (LightInfoMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture SSAOMap;
sampler SSAOSampler = sampler_state
{
    Texture = (SSAOMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 diffuseColor = tex2D(ColorSampler, input.TexCoord).rgb;
    float4 light = tex2D(LightSampler, input.TexCoord);
    float emission = tex2D(LightInfoSampler, input.TexCoord).b;
    float ssao = tex2D(SSAOSampler, input.TexCoord);

    float3 coloredEmission = diffuseColor * emission;
    float3 diffuseLight = max(light.rgb, coloredEmission);
    float specularLight = light.a;

    return float4((diffuseColor * diffuseLight + specularLight) * ssao, 1);
}

technique CombineFinal
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
