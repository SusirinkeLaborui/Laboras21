#include "PrecompiledHeader.h"
#include "Instancer.h"


Instancer::Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount)
:BaseInstancer(model, shader, maxObjectCount)
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

	accessing = false;
	access.notify_one();
}