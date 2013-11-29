#include "PrecompiledHeader.h"

#include "D3D.h"
#include "System.h"

extern "C"
{
	__declspec(dllexport) void SetMessageBoxCallback(MessageBoxCallback callback)
	{
		Tools::SetMessageBoxCallback(callback);
	}

	__declspec(dllexport) System* __cdecl CreateD3DContext(int width, int height, HWND parentWindow)
	{
		System* system = new System(width, height, parentWindow);		
		system->RunAsync();
		return system;
	}

	__declspec(dllexport) void _cdecl DestroyD3DContext(System*& system)
	{
		system->StopRunning();
		delete system;
		system = nullptr;
	}
}