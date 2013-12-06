#pragma once
#include "PrecompiledHeader.h"

#ifdef NULL			// Use nullptr instead. I'm looking at you, Darius!
	#undef NULL
#endif

struct Point
{
	int x;
	int y;
	Point(int x, int y) :x(x), y(y){}
};

struct InstancedMatrixType
{
	DirectX::XMFLOAT4X4 view;
	DirectX::XMFLOAT4X4 projection;
	DirectX::XMFLOAT4 cameraPos;
};

struct VertexType
{
	DirectX::XMFLOAT3 position;
	DirectX::XMFLOAT4 color;
	DirectX::XMFLOAT3 normal;

	VertexType(const DirectX::XMFLOAT3& position, const DirectX::XMFLOAT3& normal) : position(position), normal(normal), color(1.0f, 0.0f, 0.0f, 1.0f)
	{
	}
};

__declspec(align(16)) struct RenderParams
{
	DirectX::XMMATRIX view;
	DirectX::XMMATRIX projection;
	DirectX::XMFLOAT4 cameraPos;

	__declspec(align(16)) Microsoft::WRL::ComPtr<ID3D11DeviceContext> context;
};

struct BufferInfo
{
	unsigned int offset;
	unsigned int stride;
};