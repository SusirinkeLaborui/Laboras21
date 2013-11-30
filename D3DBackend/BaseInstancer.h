#pragma once
#include "Model.h"
#include "ColorInstancedShader.h"
#include "PrecompiledHeader.h"

using namespace std;
using namespace Microsoft::WRL;

template<class vt, class sh, class it>
class BaseInstancer
{
protected:
	Model<vt> &model;
	sh &shader;

	BufferInfo vertexInfo;
	BufferInfo instanceInfo;

	ComPtr<ID3D11Buffer> indexBuffer;
	ComPtr<ID3D11Buffer> instanceBuffer;
	ComPtr<ID3D11Buffer> vertexBuffer;

	const size_t maxInstanceCount;
	size_t instanceCount;
public:
	BaseInstancer(Model<vt> &model, sh &shader, size_t maxObjectCount);
	virtual ~BaseInstancer(void){}
	
	void Init(ComPtr<ID3D11Device> device);
	void Render(const RenderParams& params);

protected:
	void InitBuffers(ComPtr<ID3D11Device> device);
	void SetBuffers(ComPtr<ID3D11DeviceContext> context);
	virtual bool Update(ComPtr<ID3D11DeviceContext> context) = 0;
};

template<class vt, class sh, class it>
BaseInstancer<vt, sh, it>::BaseInstancer(Model<vt> &model, sh &shader, size_t maxObjectCount)
:model(model), shader(shader), instanceCount(0), maxInstanceCount(maxObjectCount)
{
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::Init(ComPtr<ID3D11Device> device)
{
	InitBuffers(device);
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::InitBuffers(ComPtr<ID3D11Device> device)
{
	D3D11_BUFFER_DESC vertexBufferDesc, indexBufferDesc;
	D3D11_SUBRESOURCE_DATA vertexData, indexData;

	vertexInfo.offset = 0;
	vertexInfo.stride = sizeof(vt);

	vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	vertexBufferDesc.ByteWidth = static_cast<UINT>(sizeof(vt)* model.vertices.size());
	vertexBufferDesc.CPUAccessFlags = 0;
	vertexBufferDesc.MiscFlags = 0;
	vertexBufferDesc.StructureByteStride = 0;
	vertexBufferDesc.Usage = D3D11_USAGE_IMMUTABLE;

	vertexData.pSysMem = model.vertices.data();
	vertexData.SysMemPitch = 0;
	vertexData.SysMemSlicePitch = 0;

	Assert(device->CreateBuffer(&vertexBufferDesc, &vertexData, &vertexBuffer));

	indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
	indexBufferDesc.ByteWidth = static_cast<UINT>(sizeof(int)* model.indices.size());
	indexBufferDesc.CPUAccessFlags = 0;
	indexBufferDesc.MiscFlags = 0;
	indexBufferDesc.StructureByteStride = 0;
	indexBufferDesc.Usage = D3D11_USAGE_IMMUTABLE;

	indexData.pSysMem = model.indices.data();
	indexData.SysMemPitch = 0;
	indexData.SysMemSlicePitch = 0;

	Assert(device->CreateBuffer(&indexBufferDesc, &indexData, &indexBuffer));

	D3D11_BUFFER_DESC instanceBufferDesc;

	ZeroMemory(&instanceBufferDesc, sizeof(instanceBufferDesc));
	instanceBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
	instanceBufferDesc.ByteWidth = static_cast<UINT>(sizeof(it) * maxInstanceCount);
	instanceBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	instanceBufferDesc.CPUAccessFlags = D3D10_CPU_ACCESS_WRITE;

	Assert(device->CreateBuffer(&instanceBufferDesc, NULL, &instanceBuffer));

	instanceInfo.offset = 0;
	instanceInfo.stride = sizeof(it);
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::Render(const RenderParams &params)
{
	if(!Update(params.context))
		return;
	SetBuffers(params.context);
	shader.SetShaderParametersInstanced(params);
	shader.RenderShaderInstanced(params.context, model.indices.size(), instanceCount);
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::SetBuffers(ComPtr<ID3D11DeviceContext> context)
{
	context->IASetIndexBuffer(indexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);
	context->IASetVertexBuffers(0, 1, vertexBuffer.GetAddressOf(), &vertexInfo.stride, &vertexInfo.offset);
	context->IASetVertexBuffers(1, 1, instanceBuffer.GetAddressOf(), &instanceInfo.stride, &instanceInfo.offset);
	context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);	
}