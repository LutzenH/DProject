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
    float4 diffuseColor = tex2D(ColorSampler, input.TexCoord);
    float4 light = tex2D(LightSampler, input.TexCoord);
    float emission = tex2D(LightInfoSampler, input.TexCoord).b;
    float ssao = tex2D(SSAOSampler, input.TexCoord);

    float3 coloredEmission = diffuseColor.rgb * emission;
    float3 diffuseLight = max(light.rgb, coloredEmission);
    float specularLight = light.a;

    return float4((diffuseColor * diffuseLight + specularLight) * diffuseColor.a * ssao, 1);
}

technique CombineFinal
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

float4 PixelShaderFunctionNoLights(VertexShaderOutput input) : COLOR0
{
    float4 diffuseColor = tex2D(ColorSampler, input.TexCoord);
    float ssao = tex2D(SSAOSampler, input.TexCoord);

    return float4(diffuseColor.rgb * diffuseColor.a * ssao, 1);
}

technique CombineFinalNoLights
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionNoLights();
    }
}

float4 PixelShaderFunctionNoSSAO(VertexShaderOutput input) : COLOR0
{
    float4 diffuseColor = tex2D(ColorSampler, input.TexCoord);
    float4 light = tex2D(LightSampler, input.TexCoord);
    float emission = tex2D(LightInfoSampler, input.TexCoord).b;

    float3 coloredEmission = diffuseColor.rgb * emission;
    float3 diffuseLight = max(light.rgb, coloredEmission);
    float specularLight = light.a;

    return float4((diffuseColor.rgb * diffuseLight + specularLight) * diffuseColor.a, 1);
}

technique CombineFinalNoSSAO
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionNoSSAO();
    }
}

float4 PixelShaderFunctionNoLightsNoSSAO(VertexShaderOutput input) : COLOR0
{
    float4 diffuseColor = tex2D(ColorSampler, input.TexCoord);

    return float4(diffuseColor.rgb * diffuseColor.a, 1);
}

technique CombineFinalNoLightsNoSSAO
{
    pass P0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionNoLightsNoSSAO();
    }
}
