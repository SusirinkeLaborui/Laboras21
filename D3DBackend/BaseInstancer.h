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

	vector<it> queue;
public:
	BaseInstancer(Model<vt> &model, sh &shader, size_t maxObjectCount);
	virtual ~BaseInstancer() {}
	
	void Init(ComPtr<ID3D11Device> device);
	void Render(const RenderParams& params);

	void Clear();
protected:
	void InitBuffers(ComPtr<ID3D11Device> device);
	void SetBuffers(ComPtr<ID3D11DeviceContext> context);
	bool Update(ComPtr<ID3D11DeviceContext> context);
	void Add(it item);
	void Add(vector<it> items);
};

typedef BaseInstancer<VertexType, ColorInstancedShader, pair<XMFLOAT4X4, XMFLOAT4X4>> Instancer;

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
	instanceBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

	Assert(device->CreateBuffer(&instanceBufferDesc, nullptr, &instanceBuffer));

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

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::Add(it item)
{
	queue.push_back(item);
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::Add(vector<it> items)
{
	Tools::VectorAppend(queue, items);
}

template<class vt, class sh, class it>
bool BaseInstancer<vt, sh, it>::Update(ComPtr<ID3D11DeviceContext> context)
{
	size_t count = queue.size();
	if (instanceCount + count > maxInstanceCount)
		count = maxInstanceCount - instanceCount;
	if (count > 0)
	{
		D3D11_MAPPED_SUBRESOURCE resource;
		context->Map(instanceBuffer.Get(), 0, D3D11_MAP_WRITE_NO_OVERWRITE, 0, &resource);
		auto ptr = static_cast<it*>(resource.pData);
		memcpy(ptr + instanceCount, queue.data(), sizeof(it) * count);
		context->Unmap(instanceBuffer.Get(), 0);
		instanceCount += count;
		queue.clear();
	}
	return instanceCount > 0;
}

template<class vt, class sh, class it>
void BaseInstancer<vt, sh, it>::Clear()
{
	instanceCount = 0;
	queue.clear();
}