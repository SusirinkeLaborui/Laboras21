#pragma once

#include "D3D.h"
#include "Model.h"
#include "Light.h"
#include "Edges.h"
#include "Nodes.h"
#include "Camera.h"

class Graphics
{
private:
	D3D d3D;	

	Nodes nodes;
	Edges edges;
	Camera camera;

	DirectX::XMFLOAT4 backgroundColor;
public:
	Graphics(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND windowHandle);
	~Graphics();

	void Render();
	void ResizeD3DContext(int newWidth, int newHeight) { d3D.ResizeContext(newWidth, newHeight); }

	Nodes &GetNodes(){ return nodes; }
	Edges &GetEdges(){ return edges; }
};

