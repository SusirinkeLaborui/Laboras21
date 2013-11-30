#include "PrecompiledHeader.h"

#include "D3D.h"
#include "System.h"
#include "Tools.h"

extern "C"
{
	__declspec(dllexport) void __stdcall SetMessageBoxCallback(MessageBoxCallback callback)
	{
		Tools::SetMessageBoxCallback(callback);
	}

	__declspec(dllexport) System* __stdcall CreateD3DContext(int width, int height, HWND parentWindow)
	{
		auto systemInstance = new System(width, height, parentWindow);
		systemInstance->RunAsync();
		return systemInstance;
	}

	__declspec(dllexport) void __stdcall DestroyD3DContext(System*& systemInstance)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");

		systemInstance->StopRunning();
		delete systemInstance;
		systemInstance = nullptr;
	}

	__declspec(dllexport) void __stdcall ResizeWindow(System* systemInstance, int newWidth, int newHeight)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");
	}
}