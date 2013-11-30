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
	vector<SimpleModel> models;
	ColorInstancedShader shader;

	static vector<FaceVertex> GetVerticesFromFace(string &line);
	static FaceVertex GetVertexFromString(string &vertex);

	ResourceManager(const ResourceManager&);
	ResourceManager &operator=(const ResourceManager&);
	static ResourceManager *handle;

	static SimpleModel GetModelFromOBJ(string filename, bool invert = false);
public:
	ResourceManager(void);
	~ResourceManager(void){}

	void InitShaders(Microsoft::WRL::ComPtr<ID3D11Device>);
	SimpleModel &GetModel(int model){ return models[model]; }
	static ResourceManager &Get(){ return *handle; }

	ColorInstancedShader &GetShader(){ return shader; }

	enum Models
	{
		MODEL_CIRCLE,
		MODEL_SQUARE
	};
};

typedef ResourceManager RM;