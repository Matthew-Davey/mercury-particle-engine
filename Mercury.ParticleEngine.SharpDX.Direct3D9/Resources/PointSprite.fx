uniform extern float4x4 WVPMatrix;
uniform extern texture SpriteTexture;
uniform extern bool FastFade;

sampler Sampler = sampler_state
{
    Texture = <SpriteTexture>;
    
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = POINT;
    
    AddressU = CLAMP;
    AddressV = CLAMP;
};                        

float4 vertex_main(const in float age : COLOR1, const in float2 position : POSITION0, inout float4 color : COLOR0, inout float size : PSIZE0, const inout float rotation : COLOR2) : POSITION0
{
	if (FastFade) {
		color.a = 1.0f - age;
	}

	return mul(float4(position, 0, 1), WVPMatrix);
}

float4 pixel_main(in float4 color : COLOR0, in float rotation : COLOR2, in float2 texCoord : TEXCOORD0) : COLOR0
{
	float c = cos(rotation);
	float s = sin(rotation);
	
	float2x2 rotationMatrix = float2x2(c, -s, s, c);
	
	texCoord = mul(texCoord - 0.5f, rotationMatrix);
	
	return tex2D(Sampler, texCoord + 0.5) * color;
}

technique PointSprite_2_0
{
    pass P0
    {
        vertexShader = compile vs_3_0 vertex_main();
        pixelShader = compile ps_3_0 pixel_main();
    }
}