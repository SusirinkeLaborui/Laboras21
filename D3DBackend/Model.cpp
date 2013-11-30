#include "PrecompiledHeader.h"
#include "Model.h"


Model::Model(ComPtr<ID3D11Device> device, wstring objPath, const Light& light) :
	light(light)
{
	throw exception("Not implemented!");
}


Model::~Model()
{
}


void Model::Render(ComPtr<ID3D11DeviceContext> deviceContext, const DirectX::XMMATRIX& transformMatrix, vector<InstanceData> instanceData)
{
	throw exception("Not implemented!");
}