#pragma once
#include "BaseInstancer.h"
class Instancer : public BaseInstancer<VertexType, ColorInstancedShader, XMFLOAT4X4>
{
	vector<XMFLOAT4X4> data;
	mutex mtx;
	condition_variable access;
	volatile bool accessing;
public:
	Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount);
	~Instancer();

	void Add(XMFLOAT4X4);
	bool Update(ComPtr<ID3D11DeviceContext> context);
};