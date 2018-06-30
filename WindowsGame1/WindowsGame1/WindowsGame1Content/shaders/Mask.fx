sampler s0;
texture sprite;
sampler spriteSampler = sampler_state 
{ 
	Texture = sprite;
	AddressU = Clamp;
	AddressV = Clamp;
};

float kernel[13] = {
	0.068786,
	0.072672,
	0.076014,
	0.078719,
	0.08071,
	0.081929,
	0.082339,
	0.081929,
	0.08071,
	0.078719,
	0.076014,
	0.072672,
	0.068786
};

float pixelWidth;

float4 PixelShaderFunction(float2 coords : TEXCOORD0 ) : COLOR0
{
	float4 color = tex2D(spriteSampler, coords);
	if (tex2D(s0, coords).b > 0.2f)
		color.a = 0;

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
