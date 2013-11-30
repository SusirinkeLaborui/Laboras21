#include "PrecompiledHeader.h"
#include "Graphics.h"


Graphics::Graphics(int windowWidth, int windowHeight, HWND windowHandle) :
	d3D(windowWidth, windowHeight, windowHandle)
{
}

Graphics::~Graphics()
{
}


void Graphics::Render()
{
	d3D.StartDrawing(1.0f, 0.0f, 0.0f, 1.0f);


	d3D.SwapBuffers();
}