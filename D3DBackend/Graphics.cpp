#include "PrecompiledHeader.h"
#include "Graphics.h"


Graphics::Graphics(int windowWidth, int windowHeight, HWND windowHandle) :
	d3D(windowWidth, windowHeight, windowHandle),
	light(DirectX::XMFLOAT3(-1.0f, -1.0f, 0.0f), DirectX::XMFLOAT4(1.0f, 1.0f, 1.0f, 1.0f), DirectX::XMFLOAT4(0.2f, 0.2f, 0.2f, 1.0f))
 	//nodeModel(d3D.GetDevice(), L"Models\\Node.obj", light),
	//edgeModel(d3D.GetDevice(), L"Models\\Edge.obj", light)
{
}

Graphics::~Graphics()
{
}


void Graphics::Render(vector<InstanceData> nodeData, vector<InstanceData> edgeData)
{
	d3D.StartDrawing(1.0f, 0.0f, 0.0f, 1.0f);


	d3D.SwapBuffers();
}