float4 ambientColor,lightColor;
float3 spritePosition,lightPosition;
float lightStrength,lightDecay,lightRadius;
float2 imageSize;

Texture SpecularMap;
sampler SpecularMapSampler = sampler_state {
	texture = <SpecularMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture ShadowMap;
sampler ShadowMapSampler = sampler_state {
	texture = <ShadowMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture NormalMap;
sampler NormalMapSampler = sampler_state {
	texture = <NormalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture BackgroundMap;
float rotation;
sampler BackgroundMapSampler = sampler_state {
	texture = <BackgroundMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture EntityMap;
sampler EntityMapSampler = sampler_state {
	texture = <EntityMap>;
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

float3 GetPixelPosition(VertexToPixel PSIn) 
{
	return float3((PSIn.TexCoord.x * imageSize.x) + spritePosition.x - (imageSize.x/2),(PSIn.TexCoord.y * imageSize.y) + spritePosition.y- (imageSize.y/2),0);
} 
VertexToPixel SimpleVS(float4 inPos: POSITION0, float2 texCoord: TEXCOORD0, float4 color: COLOR0)
{
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.TexCoord = texCoord;
	Output.Color = color;
	
	return Output;
}

PixelToFrame PointLightPS(VertexToPixel PSIn): COLOR0
{	
	PixelToFrame Output = (PixelToFrame)0;
	float4 colorMap;
	if (tex2D(EntityMapSampler, PSIn.TexCoord).a > 0.85)
		colorMap = tex2D(EntityMapSampler, PSIn.TexCoord);
	else
	{
		float4 shadowCol = tex2D(ShadowMapSampler, PSIn.TexCoord);
		colorMap.rgb = shadowCol.rgb * shadowCol.a + tex2D(BackgroundMapSampler, PSIn.TexCoord) * (1-shadowCol.a);
		colorMap.a = 1.0f;
	}

	float specularStrength =  dot(tex2D(SpecularMapSampler, PSIn.TexCoord), float3(0.3, 0.59, 0.11));
		
	float3 normal = tex2D(NormalMapSampler, PSIn.TexCoord);
		
	float3 pixelPosition = GetPixelPosition(PSIn);

	float3 lightDirection = lightPosition - pixelPosition;

	float coneAttenuation = saturate(1.0f - length(lightDirection) / lightDecay); 
				
	float specular = pow(saturate(dot(float3(0, 0, 1),normal)),specularStrength * 100);
	float4 halfMap = float4(colorMap.x *0.5f,colorMap.y *0.5f,colorMap.z *0.5f,colorMap.w);

	Output.Color =  halfMap*ambientColor + halfMap * coneAttenuation * lightColor * lightStrength + (specular * coneAttenuation  *lightColor);
	//Output.Color.rgb = Output.Color.rgb *saturate(float3(sin(0.1+ time),cos(0.21+ time), tan(0.3+ time))) ;
	return Output;
}

PixelToFrame ShadowMapPS(VertexToPixel PSIn) : COLOR0
{	
	PixelToFrame Output = (PixelToFrame)0;
	float4 entityColor = tex2D(EntityMapSampler,PSIn.TexCoord);

	float3 pixelPosition = GetPixelPosition(PSIn);
	float2 lightCoords = lightPosition.xy / imageSize;
	float3 dist = length(lightPosition - pixelPosition);
    
		
	// Cast a line from pos to light, if it hits somthing, render this one off..	
	float MaxStep = 0.02f;
	float stepSize = 0.002f;
	
	Output.Color = float4(0.1f,0.1f,0.1f,0.0f);
	for(float p = 0;p < MaxStep; p+= stepSize)
		if(tex2D(EntityMapSampler,float2(lerp(lightCoords,PSIn.TexCoord,1 / (p+1)))).a >0.85f)
			Output.Color = float4(0.01f,0.01f,0.01f,0.85f);
    return Output;
}

float4 BlurPS(VertexToPixel PSIn): COLOR0
{
	float PI = 3.14159;
	int BLUR_STEPS = 10;
	float BLUR_STEP_SIZE = 0.005;
	float4 Color = float4(0,0,0,0);
	Color = tex2D(ShadowMapSampler,PSIn.TexCoord.xy);
	for (float i = 0; i < PI; i += PI/BLUR_STEPS)
		Color += tex2D(ShadowMapSampler,PSIn.TexCoord.xy + float2(BLUR_STEP_SIZE * cos(i),BLUR_STEP_SIZE* sin(i)));

	Color = Color/BLUR_STEPS;
	return Color;
}

technique ShadowMapRender
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 SimpleVS();
        PixelShader = compile ps_3_0 ShadowMapPS();
    }
}

technique Blur
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 SimpleVS();
        PixelShader = compile ps_3_0 BlurPS();
    }
}

technique PointLightRender
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 SimpleVS();
        PixelShader = compile ps_3_0 PointLightPS();
    }
}

