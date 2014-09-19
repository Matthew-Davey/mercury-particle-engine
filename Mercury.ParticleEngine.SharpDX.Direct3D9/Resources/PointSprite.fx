uniform extern float4x4 WVPMatrix;
uniform extern texture SpriteTexture;
uniform extern bool FastFade;

sampler Sampler = sampler_state {
    Texture = <SpriteTexture>;
    
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = POINT;
    
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float3 HueToRgb(in float hue) {
	float r = abs(hue * 6 - 3) - 1;
	float g = 2 - abs(hue * 6 - 2);
	float b = 2 - abs(hue * 6 - 4);

	return saturate(float3(r, g, b));
}

float3 HslToRgb(in float3 hsl) {
	float3 rgb = HueToRgb(hsl.x / 360.0f);
	float c = (1 - abs(2 * hsl.z - 1)) * hsl.y;

	return (rgb - 0.5) * c + hsl.z;
}

float4 vertex_main(const in float age : COLOR1, const in float2 position : POSITION0, inout float4 color : COLOR0, inout float size : PSIZE0, const inout float rotation : COLOR2) : POSITION0 {
	color.xyz = HslToRgb(color.xyz);

	if (FastFade) {
		color.a = 1.0f - age;
	}

	return mul(float4(position, 0, 1), WVPMatrix);
}

float4 pixel_main(in float4 color : COLOR0, in float rotation : COLOR2, in float2 texCoord : TEXCOORD0) : COLOR0 {
	float c = cos(rotation);
	float s = sin(rotation);
	
	float2x2 rotationMatrix = float2x2(c, -s, s, c);
	
	texCoord = mul(texCoord - 0.5f, rotationMatrix);
	
	return tex2D(Sampler, texCoord + 0.5) * color;
}

technique PointSprite_2_0 {
    pass P0     {
		CullMode = NONE;
		PointSpriteEnable = TRUE;
		PointScaleEnable = FALSE;
		PointSize_Min = 1.0f;
		PointSize_Max = 1024.0f;

		AlphaBlendEnable = TRUE;
		ZWriteEnable = FALSE;

        vertexShader = compile vs_3_0 vertex_main();
        pixelShader = compile ps_3_0 pixel_main();
    }
}