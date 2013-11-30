cbuffer TransMatrix : register(b0)
{
	matrix view;
	matrix projection;
};

struct VertexInputType
{
	float4 position : POSITION0;
	float4 color : COLOR;
	float4x4 world : INSTANCE;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PixelInputType main(VertexInputType input)
{
	PixelInputType output;

	input.position.w = 1.0f;
	output.position = mul(input.position, input.world);
	output.position = mul(output.position, view);
	output.position = mul(output.position, projection);

	output.color = input.color;

	return output;
}