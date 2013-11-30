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

#pragma once

#include "Globals.h"

using namespace std;

template<class T>
struct Model
{
	vector<T> vertices;
	vector<int> indices;

	Model(){}
	Model(Model &&other);
	Model &operator=(Model &&other);

	Model(Model&) = delete;
	Model &operator=(Model&) = delete;
};

typedef Model<VertexType> SimpleModel;

template<class T>
Model<T>::Model(Model &&other)
	:vertices(move(other.vertices)), indices(move(other.indices))
{
	DirectX::XMFLOAT2 temp(hitbox);
	hitbox = other.hitbox;
	other.hitbox = temp;
}

template<class T>
Model<T> &Model<T>::operator=(Model &&other)
{
	if(this != &other)
	{
		vertices = move(other.vertices);
		indices = move(other.indices);
		DirectX::XMFLOAT2 temp(hitbox);
		hitbox = other.hitbox;
		other.hitbox = temp;
	}
	return *this;
}
