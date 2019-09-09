/*
Modified MIT License (MIT)

Copyright (c) 2015 Completely Fair Games Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

The following content pieces are considered PROPRIETARY and may not be used
in any derivative works, commercial or non commercial, without explicit 
written permission from Completely Fair Games:

* Images (sprites, textures, etc.)
* 3D Models
* Sound Effects
* Music

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#define FXAA_HLSL_3 1

float4x4 World;
float4x4 View;
float4x4 Projection;

texture TheTexture : register(t0);
sampler TheSampler : register(s0) = sampler_state
{
	Texture = <TheTexture>;
};

#ifdef XBOX
#define FXAA_360 1
#else
#define FXAA_PC 1
#endif
#define FXAA_GREEN_AS_LUMA 1

#include "FXAAShader.fxh"

struct VertexShaderInput
{
	float4 Position : POSITION0;
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

	float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;

	return output;
}

float2 InverseViewportSize;
float4 ConsoleSharpness;
float4 ConsoleOpt1;
float4 ConsoleOpt2;
float SubPixelAliasingRemoval;
float EdgeThreshold;
float EdgeThresholdMin;
float ConsoleEdgeSharpness;

float ConsoleEdgeThreshold;
float ConsoleEdgeThresholdMin;

// Must keep this as constant register instead of an immediate
float4 Console360ConstDir = float4(1.0, -1.0, 0.25, -0.25);

float4 PixelShaderFunction_FXAA(in float2 texCoords : TEXCOORD0) : COLOR0
{
	float4 theSample = tex2D(TheSampler, texCoords);

	float4 value = FxaaPixelShader(
	texCoords,
	0,	// Not used in PC or Xbox 360
    TheSampler,
    TheSampler,			// *** TODO: For Xbox, can I use additional sampler with exponent bias of -1
    TheSampler,			// *** TODO: For Xbox, can I use additional sampler with exponent bias of -2
	InverseViewportSize,	// FXAA Quality only
	ConsoleSharpness,		// Console only
	ConsoleOpt1,
	ConsoleOpt2,
	SubPixelAliasingRemoval,	// FXAA Quality only
	EdgeThreshold,// FXAA Quality only
	EdgeThresholdMin,
	ConsoleEdgeSharpness,
	ConsoleEdgeThreshold,	// TODO
	ConsoleEdgeThresholdMin, // TODO
	Console360ConstDir
	);

	return value;
}

technique FXAA
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction_FXAA();
	}
}
