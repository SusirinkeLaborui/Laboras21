#include "PrecompiledHeader.h"
#include "Nodes.h"
#include "Constants.h"

void Nodes::Add(Point point)
{
	BaseInstancer::Add(GetNodeMatrix(point));
}

void Nodes::Add(const Point *points, size_t count)
{
	vector<pair<XMFLOAT4X4, XMFLOAT4X4>> worldMatrices;

	for (size_t i = 0; i < count; i++)
	{
		worldMatrices.push_back(GetNodeMatrix(points[i]));
	}

	BaseInstancer::Add(worldMatrices);
}

pair<XMFLOAT4X4, XMFLOAT4X4> Nodes::GetNodeMatrix(Point p)
{
	XMMATRIX scale = XMMatrixScaling(Constants::NodeSize, Constants::NodeSize, Constants::NodeSize);
	XMMATRIX move = XMMatrixTranslation(float(p.x), float(p.y), 0.0f);

	XMFLOAT4X4 world, inversedTransposedWorld;
	XMMATRIX transposedWorld = scale * move;
	
	XMStoreFloat4x4(&world, XMMatrixTranspose(transposedWorld));
	XMStoreFloat4x4(&inversedTransposedWorld, XMMatrixInverse(nullptr, transposedWorld));

	return make_pair(world, inversedTransposedWorld);
}