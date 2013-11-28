cbuffer TransMatrix : register(b0)
{
	matrix view;
	matrix projection;
};

struct VertexInputType
{
	float4 position : POSITION0;
	float4 instancePos : POSITION1;
	float4 color : COLOR;
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
	input.position.x += input.instancePos.x;
	input.position.y += input.instancePos.y;
	input.position.z += input.instancePos.z;

	output.position = mul(input.position, view);
	output.position = mul(output.position, projection);

	output.color = input.color;

	return output;
}