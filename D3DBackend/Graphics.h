#pragma once

#include "D3D.h"
#include "Model.h"
#include "Light.h"
#include "Instancer.h"
#include "Camera.h"

class Graphics
{
private:
	D3D d3D;	

	Instancer nodes;
	Instancer edges;
	Camera camera;
public:
	Graphics(int windowWidth, int windowHeight, HWND windowHandle);
	~Graphics();

	void Render();
	void AddNode(Point point);
	void ResizeD3DContext(int newWidth, int newHeight) { d3D.ResizeContext(newWidth, newHeight); }
};

