uniform extern float4x4 WVPMatrix;
uniform extern texture SpriteTexture;

struct VS_INPUT
{
	float Age : COLOR1;
	float2 Position : POSITION0;
	float4 Color : COLOR0;
	float Size : PSIZE0;
	float Rotation : COLOR2;
};

struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float Size : PSIZE0;
	float Rotation : COLOR2;
};

struct PS_INPUT
{
	float4 Color : COLOR0;
	float Rotation : COLOR2;
    float2 TexCoord : TEXCOORD0;
};

sampler Sampler = sampler_state
{
    Texture = <SpriteTexture>;
    
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = POINT;
    
    AddressU = CLAMP;
    AddressV = CLAMP;
};                        

VS_OUTPUT vshader(VS_INPUT input)
{
	VS_OUTPUT output = (VS_OUTPUT)0;
	
	output.Position = mul(float4(input.Position, 0, 1), WVPMatrix);
	output.Color = input.Color;
	output.Size = input.Size;
	output.Rotation = (input.Rotation + 3.14159) / 6.283185;

	if (input.Age == 0.0f || input.Age > 1.0f)
	{
		output.Size = 0.0f;
	}

	return output;
}

float4 pshader(PS_INPUT input) : COLOR0
{
	float r = (input.Rotation * 6.283185) - 3.141593;
	
	float c = cos(r);
	float s = sin(r);
	
	float2x2 rotationMatrix = float2x2(c, -s, s, c);
	
	float2 texCoord = mul(input.TexCoord - 0.5f, rotationMatrix);
	
	return tex2D(Sampler, texCoord + 0.5) * input.Color;
}

technique PointSprite_2_0
{
    pass P0
    {
        vertexShader = compile vs_3_0 vshader();
        pixelShader = compile ps_3_0 pshader();
    }
}