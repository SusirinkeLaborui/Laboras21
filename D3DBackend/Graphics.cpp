#include "PrecompiledHeader.h"
#include "Graphics.h"
#include "ResourceManager.h"
#include "Constants.h"

Graphics::Graphics(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND windowHandle) :
	d3D(windowWidth, windowHeight, windowHandle),
	backgroundColor(backgroundColor),
	nodes(RM::Get().GetModel(RM::Models::MODEL_CIRCLE), RM::Get().GetShader(), Constants::NodeLimit),
	edges(RM::Get().GetModel(RM::Models::MODEL_SQUARE), RM::Get().GetShader(), Constants::EdgeLimit)
{
	RM::Get().InitShaders(d3D.GetDevice());
	edges.Init(d3D.GetDevice());
	nodes.Init(d3D.GetDevice());
	camera.Forward(-30000.0f);
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
	edges.Render(params);

	d3D.SwapBuffers();
}