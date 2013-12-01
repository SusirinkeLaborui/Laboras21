#include "PrecompiledHeader.h"
#include "Instancer.h"


Instancer::Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount)
:BaseInstancer(model, shader, maxObjectCount)
{
}


Instancer::~Instancer()
{
}

void Instancer::Add(XMFLOAT4X4 item)
{
	unique_lock<mutex> lock(mtx);

	queue.push_back(item);
}

void Instancer::Add(vector<XMFLOAT4X4> items)
{
	unique_lock<mutex> lock(mtx);

	Tools::VectorAppend(queue, items);
}

bool Instancer::Update(ComPtr<ID3D11DeviceContext> context)
{
	unique_lock<mutex> lock(mtx);

	size_t count = queue.size();
	if (instanceCount + count > maxInstanceCount)
		count = maxInstanceCount - instanceCount;
	if (count > 0)
	{
		D3D11_MAPPED_SUBRESOURCE resource;
		context->Map(instanceBuffer.Get(), 0, D3D11_MAP_WRITE_NO_OVERWRITE, 0, &resource);
		auto ptr = static_cast<XMFLOAT4X4*>(resource.pData);
		memcpy(ptr + instanceCount, queue.data(), sizeof(XMFLOAT4X4) * count);
		context->Unmap(instanceBuffer.Get(), 0);
		instanceCount += count;
		queue.clear();
	}
	return instanceCount > 0;
}

void Instancer::Clear()
{
	unique_lock<mutex> lock(mtx);

	instanceCount = 0;
	queue.clear();
}