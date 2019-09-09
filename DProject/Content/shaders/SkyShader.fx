float2 ViewportResolution;

texture DepthTexture;
sampler2D DepthSampler = sampler_state
{
	Texture = <DepthTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    float depth = tex2D(DepthSampler, input.TexCoord).r;
    clip(depth - 1);

    float2 uv = float2(input.TexCoord.x, 1 - input.TexCoord.y);
    uv.x *= ViewportResolution.x / ViewportResolution.y;

    // Atmosphere
    float atmosphere = sqrt(1.0 - uv.y);
    float3 skyColor = float3(0.2, 0.4, 0.8);
    float scatter = pow(0.8, 1.0 / 15.0); //0.8 is height of scatter 0.0 - 1.0
    scatter = 1.0 - clamp(scatter, 0.8, 1.0);
    float3 scatterColor = lerp(float3(1.0, 1.0, 1.0), float3(1.0, 0.3, 0.0) * 1.5, scatter);

    float3 atmosphereColor = lerp(skyColor, float3(scatterColor), atmosphere / 1.3);

    // Sun
    float sun = 1.0 - distance(uv, float2(0.5, 0.8)); //Position of the sun 0.0 - 1.0
    sun = clamp(sun, 0.0, 1.0);

    float glow = sun;
    glow = clamp(glow, 0.0, 1.0);

    sun = pow(sun,100.0);
    sun *= 100.0;
    sun = clamp(sun, 0.0, 1.0);

    glow = pow(glow, 6.0) * 1.0;
    glow = pow(glow, uv.y);
    glow = clamp(glow, 0.0, 1.0);

    sun *= pow(dot(uv.y, uv.y), 1.0 / 1.65);
    glow *= pow(dot(uv.y, uv.y), 1.0 / 2.0);

    sun += glow;

    float3 sunColor = float3(1.0, 0.6, 0.05) * sun;

	return float4(atmosphereColor + sunColor, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
};
