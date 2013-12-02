#pragma once

#include "PrecompiledHeader.h"
#include "Model.h"
#include "ColorInstancedShader.h"

using namespace std;

class ResourceManager
{
	struct FaceVertex
	{
		int normal;
		int tex;
		int vertex;
	};
	vector<ColorModel> models;
	ColorInstancedShader shader;

	static vector<FaceVertex> GetVerticesFromFace(string &line);
	static FaceVertex GetVertexFromString(string &vertex);

	ResourceManager(const ResourceManager&);
	ResourceManager &operator=(const ResourceManager&);
	static ResourceManager *handle;

	static ColorModel GetModelFromOBJ(wstring filename);
public:
	ResourceManager();
	~ResourceManager() {}

	void InitShaders(Microsoft::WRL::ComPtr<ID3D11Device>);
	ColorModel &GetModel(int model){ return models[model]; }
	static ResourceManager &Get(){ return *handle; }

	ColorInstancedShader &GetShader(){ return shader; }

	enum Models
	{
		MODEL_CIRCLE,
		MODEL_SQUARE
	};
};

typedef ResourceManager RM;