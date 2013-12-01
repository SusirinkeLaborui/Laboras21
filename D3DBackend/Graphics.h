#pragma once

#include "D3D.h"
#include "Model.h"
#include "Light.h"
#include "BaseInstancer.h"
#include "Camera.h"

class Graphics
{
private:
	D3D d3D;	

	Instancer nodes;
	Instancer edges;
	Camera camera;

	DirectX::XMFLOAT4 backgroundColor;
public:
	Graphics(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND windowHandle);
	~Graphics();

	void Render();
	void AddNode(Point point);
	void ResizeD3DContext(int newWidth, int newHeight) { d3D.ResizeContext(newWidth, newHeight); }
};

