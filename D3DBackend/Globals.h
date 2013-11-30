#pragma once
#include "PrecompiledHeader.h"

#ifndef NULL
	#define NULL 0
#endif

struct Point
{
	int x;
	int y;
};

struct InstancedMatrixType
{
	DirectX::XMFLOAT4X4 view;
	DirectX::XMFLOAT4X4 projection;
};

struct VertexType
{
	DirectX::XMFLOAT3 position;
	DirectX::XMFLOAT4 color;

	VertexType(float x, float y, float z) :position(x, y, z), color(1.0f, 0.0f, 0.0f, 1.0f){}
};

__declspec(align(16)) struct RenderParams
{
	DirectX::XMMATRIX reflecMatrix;
	DirectX::XMMATRIX view;
	DirectX::XMMATRIX projection;
	DirectX::XMFLOAT3 lightPos;
	float brightness;
	DirectX::XMFLOAT4 diffuseColor;
	__declspec(align(16)) Microsoft::WRL::ComPtr<ID3D11DeviceContext> context;
};

struct BufferInfo
{
	unsigned int offset;
	unsigned int stride;
};