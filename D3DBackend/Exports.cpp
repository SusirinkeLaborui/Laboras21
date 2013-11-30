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
		systemInstance->ResizeWindow(newWidth, newHeight);
	}

    __declspec(dllexport) void __stdcall DrawNodes(System* systemInstance, Point* nodeList, int nodeCount)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");
		systemInstance->DrawNodes(nodeList, nodeCount);
	}

    __declspec(dllexport) void __stdcall DrawEdge(System* systemInstance, Point nodeA, Point nodeB)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");
		systemInstance->DrawEdge(nodeA, nodeB);
	}
        
    __declspec(dllexport) void __stdcall ClearNodes(System* systemInstance)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");
		systemInstance->ClearNodes();
	}

    __declspec(dllexport) void __stdcall ClearEdges(System* systemInstance)
	{
		AssertBool(systemInstance != nullptr, L"System instance can't be null!");
		systemInstance->ClearEdges();
	}
}