#pragma once
#include "BaseInstancer.h"
class Instancer : public BaseInstancer<VertexType, ColorInstancedShader, XMFLOAT4X4>
{
	vector<XMFLOAT4X4> queue;
	mutex mtx;
public:
	Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount);
	~Instancer();

	void Add(XMFLOAT4X4 item);
	void Add(vector<XMFLOAT4X4> items);
	bool Update(ComPtr<ID3D11DeviceContext> context);
	void Clear();
};