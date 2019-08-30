float2 ViewportSize;       // The viewport size in pixels.
float2 TerrainTileOrigin;  // World space origin of this tile.
float2 TerrainTileSize;    // World space size of this tile.
float TerrainClipMapLevel; // The clipmap level which is currently being rendered.

float2 TerrainTileTextureSize;
texture TerrainTileTexture;
sampler2D terrainSampler = sampler_state
{
	Texture = <TerrainTileTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

float2 ScreenToProjection(float2 position, float2 viewportSize)
{
  position /= viewportSize;
  position *= float2(2, -2);
  position -= float2(1, -1);
  
  return position;
}

float4 ScreenToProjection(float4 position, float2 viewportSize)
{
  position.xy = ScreenToProjection(position.xy, viewportSize);
  return position;
}

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    
    output.Position = ScreenToProjection(input.Position, ViewportSize);
    
    output.TextureCoordinate = (input.TextureCoordinate - TerrainTileOrigin) / TerrainTileSize;
    output.TextureCoordinate += 0.5 / TerrainTileTextureSize;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 sample = tex2D(terrainSampler, input.TextureCoordinate);
    
	return sample;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile vs_3_0 MainVS();
		PixelShader = compile ps_3_0 MainPS();
	}
};
