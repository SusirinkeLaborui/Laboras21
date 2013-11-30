#pragma once
#include "DrawableEntity.h"
#include "Model.h"
#include "ColorInstancedShader.h"
#include "PrecompiledHeader.h"

using namespace std;
using namespace Microsoft::WRL;

template<class vt, class sh, class it>
class BaseInstancer : public DrawableEntity<vt, sh>
{
public:
	BaseInstancer(Model<vt> &model, sh &shader, size_t maxObjectCount, XMFLOAT3 pos = XMFLOAT3());
	virtual ~BaseInstancer(void){}

protected:
	const size_t maxInstanceCount;
	size_t instanceCount;

	ComPtr<ID3D11Buffer> instanceBuffer;
	BufferInfo instanceInfo;
public:
	void Init(ComPtr<ID3D11Device> device);
	void Render(const RenderParams& params);

protected:
	void InitBuffers(ComPtr<ID3D11Device> device);
	void SetBuffers(ComPtr<ID3D11DeviceContext> context);
	virtual bool Update(ComPtr<ID3D11DeviceContext> context) = 0;
};

template<class vt, class sh, class it>
BaseInstancer<vt, sh, it>::BaseInstancer(Model<vt> &model, sh &shader, size_t maxObjectCount, XMFLOAT3 pos)
:DrawableEntity(pos, model, shader), instanceCount(0), maxInstanceCount(maxObjectCount)
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
	DrawableEntity::InitBuffers(device);

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
	DrawableEntity::SetBuffers(context);
	context->IASetVertexBuffers(1, 1, instanceBuffer.GetAddressOf(), &instanceInfo.stride, &instanceInfo.offset);
}