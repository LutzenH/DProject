float4x4 World;
float4x4 View;
float4x4 Projection;

float SpecularIntensity;
float SpecularPower;

texture diffuseTexture;
sampler2D diffuseSampler = sampler_state
{
	Texture = <diffuseTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

texture heightmapTexture;
sampler2D heightmapSampler = sampler_state
{
	Texture = <heightmapTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

texture normalTexture;
sampler2D normalSampler = sampler_state
{
	Texture = <normalTexture>;
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
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float2 xy = ClipMapOffset + mul(input.Position, World).xz * ClipMapScale;
	float2 texCoord = -xy / TextureDimension;

	float2 heightSample = tex2Dlod(heightmapSampler, float4(texCoord, 0, 0)).rg;
	float ScaledHeightSample = 512 * heightSample.r + heightSample.g * 2;

	float4 elevatedPos = float4(input.Position.x, ScaledHeightSample, input.Position.z, input.Position.w);

    float4 worldPosition = mul(elevatedPos, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);

    output.TexCoord = texCoord;

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

    output.Color = tex2D(diffuseSampler, input.TexCoord);
    output.Color.a = SpecularIntensity;

    output.Normal.rgb = tex2D(normalSampler, input.TexCoord).rbg;
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
