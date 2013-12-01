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
	camera.Forward(15.0f);
	AddNode(Point(0, 0));
	AddNode(Point(5, 0));
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
	nodes.Add(GetNodeMatrix(point));
}

void Graphics::AddNodes(vector<Point> points)
{
	vector<XMFLOAT4X4> matrices;
	for (auto &point : points)
		matrices.push_back(GetNodeMatrix(point));
	nodes.Add(matrices);
}

XMFLOAT4X4 Graphics::GetNodeMatrix(Point point)
{
	XMMATRIX scale = XMMatrixScaling(1.0f, 1.0f, 1.0f);
	XMMATRIX move = XMMatrixTranslation(static_cast<float>(point.x), static_cast<float>(point.y), 0.0f);
	XMFLOAT4X4 world;
	XMStoreFloat4x4(&world, XMMatrixTranspose(scale * move));
	return world;
}