sampler s0;
texture sprite;
sampler spriteSampler = sampler_state 
{ 
	Texture = sprite;
	AddressU = Clamp;
	AddressV = Clamp;
};

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
