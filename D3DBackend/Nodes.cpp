#include "PrecompiledHeader.h"
#include "Nodes.h"

void Nodes::Add(Point point)
{
	BaseInstancer::Add(GetNodeMatrix(point));
}

void Nodes::Add(const Point *points, size_t count)
{
	vector<XMFLOAT4X4> matrices;
	for (size_t i = 0; i < count; i++)
		matrices.push_back(GetNodeMatrix(points[i]));
	BaseInstancer::Add(matrices);
}

XMFLOAT4X4 Nodes::GetNodeMatrix(Point p)
{
	XMMATRIX scale = XMMatrixScaling(1.0f, 1.0f, 1.0f);
	XMMATRIX move = XMMatrixTranslation(static_cast<float>(p.x), static_cast<float>(p.y), 0.0f);
	XMFLOAT4X4 world;
	XMStoreFloat4x4(&world, XMMatrixTranspose(scale * move));
	return world;
}