#pragma once

struct InstanceData
{
	DirectX::XMMATRIX worldMatrix;
	DirectX::XMFLOAT4 color;
};

class Light;
class Model
{
private:
	ComPtr<ID3D11Buffer> vertexBuffer;
	ComPtr<ID3D11Buffer> indexBuffer;
	ComPtr<ID3D11Buffer> instanceBuffer;

	int indexCount;
	const Light& light;

	Model(const Model& other);
public:
	Model(ComPtr<ID3D11Device> device, wstring objPath, const Light& light);
	~Model();

	void Render(ComPtr<ID3D11DeviceContext> deviceContext, const DirectX::XMMATRIX& transformMatrix, vector<InstanceData> instanceData);
};

