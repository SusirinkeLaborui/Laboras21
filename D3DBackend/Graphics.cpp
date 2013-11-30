#include "PrecompiledHeader.h"
#include "Graphics.h"
#include "ResourceManager.h"

Graphics::Graphics(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND windowHandle) :
	d3D(windowWidth, windowHeight, windowHandle),
	backgroundColor(backgroundColor),
	nodes(RM::Get().GetModel(RM::Models::MODEL_CIRCLE), RM::Get().GetShader(), 10000),
	edges(RM::Get().GetModel(RM::Models::MODEL_SQUARE), RM::Get().GetShader(), 10000)
{
	RM::Get().InitShaders(d3D.GetDevice());
	edges.Init(d3D.GetDevice());
	nodes.Init(d3D.GetDevice());
	camera.Forward(-100.0f);
}

Graphics::~Graphics()
{
}


void Graphics::Render()
{
	d3D.StartDrawing(backgroundColor.x, backgroundColor.y, backgroundColor.z, backgroundColor.w);

	RenderParams params;
	params.view = camera.GetViewMatrix();
	params.projection = d3D.GetProjectionMatrix();
	params.context = d3D.GetDeviceContext();

	nodes.Render(params);

	d3D.SwapBuffers();
}

void Graphics::AddNode(Point point)
{
	XMMATRIX scale = XMMatrixScaling(200.0f, 200.0f, 200.0f);
	XMMATRIX move = XMMatrixTranslation(static_cast<float>(point.x), static_cast<float>(point.y), 0.0f);
	XMFLOAT4X4 world;
	XMStoreFloat4x4(&world, XMMatrixTranspose(scale * move));
	nodes.Add(world);
}