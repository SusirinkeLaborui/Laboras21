#include "PrecompiledHeader.h"
#include "ResourceManager.h"
using namespace Microsoft::WRL;

ResourceManager *ResourceManager::handle;

ResourceManager::ResourceManager()
:shader(L"ColorInstancedVertex.cso", L"ColorInstancedPixel.cso")
{
	handle = this;
	models.push_back(GetModelFromOBJ(L"Resources\\Node.obj"));
	models.push_back(GetModelFromOBJ(L"Resources\\Edge.obj"));
}

ColorModel ResourceManager::GetModelFromOBJ(wstring filename)
{
	ColorModel model;
	ifstream in(filename, ios::binary);
	AssertBool(in.is_open(), L"Couldn't open file " + filename);

	string input;
	float x, y, z;
	map<VertexType, int> vertexMap;

	vector<DirectX::XMFLOAT3> positions, normals;

	while (!in.eof())
	{
		in >> input;

		if (input == "#")
		{
			in.ignore(200, '\n');
			continue;
		}

		if (input == "v")
		{
			in >> x >> y >> z;

			if (!in.fail())
			{
				positions.emplace_back(x, y, z);
			}
		}
		else if (input == "vn")
		{
			in >> x >> y >> z;
			if (!in.fail())
			{
				normals.emplace_back(x, y, z);
			}
		}
		else if (input == "f")
		{
			string blob;
			getline(in, blob, '\n');
			if (!in.fail())
			{
				auto vertices = GetVerticesFromFace(blob);
				Tools::Reverse(vertices);

				for (auto &vertex : vertices)
				{
					InsertVertex(vertexMap, positions[vertex.vertex], normals[vertex.normal], model.vertices, model.indices);

					//model.indices.push_back(model.vertices.size());
					//model.vertices.emplace_back(positions[vertex.vertex], normals[vertex.normal]);
				}
			}
		}
	}

	return model;
}


vector<ResourceManager::FaceVertex> ResourceManager::GetVerticesFromFace(string &line)
{
	vector<FaceVertex> ret;
	size_t ind1 = 0, ind2 = 0;

	for(int i = 0; i < 3; i++)
	{
		ind2 = line.find(' ', ind1+1);
		ret.push_back(GetVertexFromString(line.substr(ind1, ind2-ind1)));
		ind1 = ++ind2;
	}
	return ret;
}

ResourceManager::FaceVertex ResourceManager::GetVertexFromString(string &vertex)
{
	FaceVertex ret;
	string temp;
	size_t ind1 = 0, ind2 = 0;

	ind1 = vertex.find('/');
	temp = vertex.substr(0, ind1);
	ret.vertex = temp.length() > 0 ? stoi(temp) - 1 : -1;
	ind2 = vertex.find('/', ind1+1);
	temp = vertex.substr(ind1+1, ind2-ind1-1);
	ret.tex = temp.length() > 0 ? stoi(temp) - 1 : -1;
	temp = vertex.substr(ind2+1);
	ret.normal = temp.length() > 0 ? stoi(temp) - 1 : -1;

	return ret;
}


void ResourceManager::InsertVertex(map<VertexType, int>& vertexMap, const DirectX::XMFLOAT3& position, const DirectX::XMFLOAT3& normal, 
	vector<VertexType>& vertices, vector<int>& indices)
{
	VertexType vertex(position, normal);

	auto vertexIterator = vertexMap.find(vertex);

	if (vertexIterator == vertexMap.end())
	{
		vertexIterator = vertexMap.insert(make_pair(vertex, static_cast<int>(vertices.size()))).first;
		vertices.push_back(vertex);
	}
	
	indices.push_back(vertexIterator->second);
}


void ResourceManager::InitShaders(ComPtr<ID3D11Device> device)
{
	shader.Init(device);
}