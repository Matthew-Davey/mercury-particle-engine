uniform extern float4x4 WVPMatrix;
uniform extern texture SpriteTexture;

struct VS_INPUT
{
	float Age : TEXCOORD1;
	float2 Position : POSITION0;
	float4 Color : COLOR0;
	float Size : PSIZE0;
	//float Rotation : COLOR1;
};

struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float Size : PSIZE0;
	float Rotation : COLOR1;
};

struct PS_INPUT
{
	float4 Color : COLOR0;
	float Rotation : COLOR1;
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
	//output.Rotation = input.Rotation;
	output.Rotation = 0.0f;

	if (input.Age == 0.0f || input.Age > 1.0f)
	{
		output.Size = 0.0f;
	}

	return output;
}

float4 pshader(PS_INPUT input) : COLOR0
{
	float2 texCoord;
	
	float2 cCoord = input.TexCoord;
	
	cCoord += 0.5f;
	
	float ca = cos(input.Rotation);
	float sa = sin(input.Rotation);
	
	texCoord.x = cCoord.x * ca - cCoord.y * sa;
	texCoord.y = cCoord.x * sa + cCoord.y * ca;
	
	texCoord -= 0.5f;

    return tex2D(Sampler, texCoord) * input.Color;
}

technique PointSprite_2_0
{
    pass P0
    {
        vertexShader = compile vs_3_0 vshader();
        pixelShader = compile ps_3_0 pshader();
    }
}