#include "PrecompiledHeader.h"
#include "Edges.h"
#include "Constants.h"

void Edges::Add(Point a, Point b)
{
	BaseInstancer::Add(GetEdgeMatrix(a, b));
}


void Edges::AddBatch(pair<Point, Point>* points, int count)
{
	for (int i = 0; i < count; i++)
	{
		Add(points[i].first, points[i].second);
	}
}


pair<XMFLOAT4X4, XMFLOAT4X4> Edges::GetEdgeMatrix(Point a, Point b)
{
	if (a.x < b.x)
	{
		auto temp = a;
		a = b;
		b = temp;
	}
	XMVECTOR start = XMVectorSet(float(a.x), float(a.y), 0.0f, 0.0f);
	XMVECTOR end = XMVectorSet(float(b.x), float(b.y), 0.0f, 0.0f);
	XMVECTOR edge = end - start;
	XMVECTOR up = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);
	float angle;
	XMStoreFloat(&angle, XMVector2AngleBetweenVectors(up, edge));
	float length;
	XMStoreFloat(&length, XMVector2Length(edge));
	XMMATRIX scale = XMMatrixScaling(Constants::EdgeWidth, length, Constants::EdgeWidth);
	XMMATRIX rot = XMMatrixRotationZ(angle);
	XMMATRIX pos = XMMatrixTranslation((a.x + b.x) / 2.0f, (a.y + b.y) / 2.0f, 0.0f);

	XMFLOAT4X4 world, inversedTransposedWorld;
	XMMATRIX transposedWorld = scale * rot * pos;
	
	XMStoreFloat4x4(&world, XMMatrixTranspose(transposedWorld));
	XMStoreFloat4x4(&inversedTransposedWorld, XMMatrixInverse(nullptr, transposedWorld));
	
	return make_pair(world, inversedTransposedWorld);
}