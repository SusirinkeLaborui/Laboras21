#include "PrecompiledHeader.h"
#include "Instancer.h"


Instancer::Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount)
:BaseInstancer(model, shader, maxObjectCount), accessing(false)
{
}


Instancer::~Instancer()
{
}

void Instancer::Add(XMFLOAT4X4 stuff)
{
	unique_lock<mutex> lock(mtx);
	access.wait(lock, [=]{ return !accessing; });
	accessing = true;
	if (instanceCount != maxInstanceCount)
	{
		instanceCount++;
		data.push_back(stuff);
	}
	accessing = false;
	access.notify_one();
}

bool Instancer::Update(ComPtr<ID3D11DeviceContext> context)
{
	unique_lock<mutex> lock(mtx);
	access.wait(lock, [=]{ return !accessing; });
	accessing = true;

	D3D11_MAPPED_SUBRESOURCE resource;
	context->Map(instanceBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &resource);
	memcpy(resource.pData, data.data(), sizeof(XMFLOAT4X4) * instanceCount);
	context->Unmap(instanceBuffer.Get(), 0);

	accessing = false;
	access.notify_one();
	return instanceCount > 0;
}