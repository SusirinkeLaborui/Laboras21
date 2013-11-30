#pragma once

#include "D3D.h"
#include "Model.h"
#include "Light.h"
#include "Instancer.h"

class Graphics
{
private:
	D3D d3D;	

	Instancer nodes;
	Instancer edges;

public:
	Graphics(int windowWidth, int windowHeight, HWND windowHandle);
	~Graphics();

	void Render();
	void AddNode(Point point);
};

