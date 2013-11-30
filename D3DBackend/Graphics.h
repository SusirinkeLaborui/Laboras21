#pragma once

#include "D3D.h"
#include "Model.h"
#include "Light.h"

class Graphics
{
private:
	D3D d3D;
	
	Light light;

	//Model nodeModel;
	//Model edgeModel;	

public:
	Graphics(int windowWidth, int windowHeight, HWND windowHandle);
	~Graphics();

	void Render(vector<InstanceData> nodeData, vector<InstanceData> edgeData);
};

