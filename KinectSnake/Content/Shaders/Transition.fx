
float TransitionPosition;
Texture SceneMap;
sampler SceneMapSampler = sampler_state {
	texture = <SceneMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture TransitionMap;
sampler TransitionMapSampler = sampler_state {
	texture = <TransitionMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};


struct VertexToPixel
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

VertexToPixel SimpleVS(float4 inPos: POSITION0, float2 texCoord: TEXCOORD0, float4 color: COLOR0)
{
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.TexCoord = texCoord;
	Output.Color = color;
	
	return Output;
}


PixelToFrame TransitionPS(VertexToPixel PsIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	
	float2 transitionOffset = tex2D(TransitionMapSampler, PsIn.TexCoord).xy - 0.5;

    transitionOffset *= TransitionPosition * 0.25;

    Output.Color = float4(tex2D(SceneMapSampler, PsIn.TexCoord + transitionOffset).rgb,1.0f);

	return Output;
}


technique Transition
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 SimpleVS();
        PixelShader = compile ps_3_0 TransitionPS();
    }
}