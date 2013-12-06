cbuffer TransMatrix : register(b0)
{
	matrix view;
	matrix projection;
    float4 cameraPosition;
};

struct VertexInputType
{
	float4 position : POSITION;
	float4 color : COLOR;
	float4 normal : NORMAL;
	float4x4 world : WORLD_INSTANCED;
	float4x4 invertedTransposedWorld : INVERTEDTRANSPOSEDWORLD_INSTANCED;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float3 normal : NORMAL;
	float3 viewDirection : VIEWDIRECTION;
};

PixelInputType main(VertexInputType input)
{
	PixelInputType output;

	input.position.w = 1.0f;
	input.normal.w = 1.0f;

	output.position = mul(input.position, input.world);
	output.viewDirection = cameraPosition.xyz - output.position.xyz;

	output.position = mul(output.position, view);
	output.position = mul(output.position, projection);

    output.normal = -mul(input.normal, input.invertedTransposedWorld).xyz;

	output.color = input.color;

	return output;
}