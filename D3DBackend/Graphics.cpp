#include "PrecompiledHeader.h"
#include "Graphics.h"
#include "ResourceManager.h"

Graphics::Graphics(int windowWidth, int windowHeight, HWND windowHandle) :
	d3D(windowWidth, windowHeight, windowHandle),
	nodes(RM::Get().GetModel(RM::Models::MODEL_CIRCLE), RM::Get().GetShader(), 10000),
	edges(RM::Get().GetModel(RM::Models::MODEL_SQUARE), RM::Get().GetShader(), 10000)
{
	RM::Get().InitShaders(d3D.GetDevice());
}

Graphics::~Graphics()
{
}


void Graphics::Render()
{
	d3D.StartDrawing(1.0f, 0.0f, 0.0f, 1.0f);

	RenderParams params;

	//nodes.Render(params);

	d3D.SwapBuffers();
}

void Graphics::AddNode(Point point)
{
	XMMATRIX move = XMMatrixTranslation(static_cast<float>(point.x), static_cast<float>(point.y), 0.0f);
	XMFLOAT4X4 world;
	XMStoreFloat4x4(&world, XMMatrixTranspose(move));
	nodes.Add(world);
}