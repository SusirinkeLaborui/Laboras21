#include "PrecompiledHeader.h"

#include "System.h"
#include "Tools.h"
#include "Constants.h"

extern "C"
{
	__declspec(dllexport) void __stdcall SetMessageBoxCallback(MessageBoxCallback callback)
	{
		Tools::SetMessageBoxCallback(callback);
	}

	__declspec(dllexport) System* __stdcall CreateD3DContext(int width, int height, int backgroundR, int backgroundG, int backgroundB, HWND parentWindow)
	{
		DirectX::XMFLOAT4 backgroundColor(backgroundR / 255.0f, backgroundG / 255.0f, backgroundB / 255.0f, 1.0f);
		auto systemInstance = new System(width, height, backgroundColor, parentWindow);
		return systemInstance;
	}
    
	__declspec(dllexport) void __stdcall RunD3DContextAsync(System* systemInstance)
	{
		systemInstance->RunAsync();
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

    __declspec(dllexport) void __stdcall DrawSingleEdge(System* systemInstance, Point nodeA, Point nodeB)
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

	__declspec(dllexport) HWND __stdcall CreateColoredWindow(HWND parent, int r, int g, int b)
	{	
		WNDCLASSEX windowInfo;

		auto programInstance = GetModuleHandle(NULL);

		ZeroMemory(&windowInfo, sizeof(WNDCLASSEX));
		windowInfo.style = CS_HREDRAW | CS_VREDRAW | CS_PARENTDC;
		
		windowInfo.lpfnWndProc = [](HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) -> LRESULT
		{
			return DefWindowProc(hwnd, uMsg, wParam, lParam);
		};

		windowInfo.hInstance = programInstance;
		windowInfo.hIcon = LoadIcon(NULL, IDI_WINLOGO);
		windowInfo.hIconSm = windowInfo.hIcon;
		windowInfo.hCursor = LoadCursor(NULL, IDC_ARROW);
		windowInfo.hbrBackground = CreateSolidBrush(RGB(r, g, b));
		windowInfo.lpszMenuName = NULL;
		windowInfo.lpszClassName = Constants::ApplicationName.c_str();
		windowInfo.cbSize = sizeof(WNDCLASSEX);
	
		// Register the window class.
		RegisterClassEx(&windowInfo);

		auto windowHandle = CreateWindowEx(0, windowInfo.lpszClassName, L"", WS_CHILD, 0, 0, 0, 0, parent, nullptr, programInstance, nullptr);
		AssertBool(windowHandle != nullptr, L"Window creation failed!");

		return windowHandle;
	}
}