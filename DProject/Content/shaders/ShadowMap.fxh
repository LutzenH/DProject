float4x4 LightView;
float4x4 LightProjection;

float ShadowStrength;
float ShadowBias;

float ShadowMapSize;

texture ShadowMap;
sampler2D shadowSampler = sampler_state
{
    Texture = (ShadowMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Border;
    AddressV = Border;
};

float CalculateShadow(float4 worldPosition)
{		
    float4x4 LightViewProj = mul(LightView, LightProjection);
    float4 lightingPosition = mul(worldPosition, LightViewProj);
    
    // Get the position in the ShadowMap for this texel
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2(0.5, 0.5);
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    float shadowStrength = 0.0f;
    float2 texelSize = float2(1.0 / ShadowMapSize, 1.0 / ShadowMapSize);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            // Get the current depth stored in ShadowMap
            float shadowdepth = 1.0 - tex2D(shadowSampler, ShadowTexCoord + float2(x, y) * texelSize).r;
            float ourdepth = (lightingPosition.z / lightingPosition.w) - ShadowBias;
            
            shadowStrength += ourdepth - ShadowBias > shadowdepth ? ShadowStrength : 0.0;        
        }    
    }
    
    return 1 - shadowStrength / 9.0;
}
