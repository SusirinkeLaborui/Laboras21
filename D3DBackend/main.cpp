#include "PrecompiledHeader.h"

#include "D3D.h"

static shared_ptr<D3D> d3D;

extern "C"
{
	__declspec(dllexport) void __cdecl CreateD3DContext(int width, int height, HWND windowHandle)
	{
		d3D = make_shared<D3D>(width, height, windowHandle);

		thread([]()
		{
			while (true)
			{
				d3D->StartDrawing(1.0f, 0.0f, 0.0f, 1.0f);
				d3D->SwapBuffers();
			}
		}).detach();
		//return d3D->GetSwapChain().Get();
	}
}