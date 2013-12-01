#include "PrecompiledHeader.h"

#include "Windowing.h"
#include "System.h"
#include "Tools.h"
#include "Constants.h"

Windowing::Windowing(int windowWidth, int windowHeight, HWND parentWindow)
{
	WNDCLASSEX windowInfo;

	auto programInstance = GetModuleHandle(NULL);

	ZeroMemory(&windowInfo, sizeof(WNDCLASSEX));
	windowInfo.style = CS_HREDRAW | CS_VREDRAW | CS_PARENTDC;
	windowInfo.hInstance = programInstance;
	windowInfo.hIcon = LoadIcon(NULL, IDI_WINLOGO);
	windowInfo.hIconSm = windowInfo.hIcon;
	windowInfo.hCursor = LoadCursor(NULL, IDC_ARROW);
	windowInfo.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
	windowInfo.lpszMenuName = NULL;
	windowInfo.lpszClassName = Constants::ApplicationName.c_str();
	windowInfo.cbSize = sizeof(WNDCLASSEX);
	
	windowInfo.lpfnWndProc = [](HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam)
	{
		return DefWindowProc(windowHandle, message, wParam, lParam);
	};


	// Register the window class.
	RegisterClassEx(&windowInfo);

	windowHandle = CreateWindowEx(0, windowInfo.lpszClassName, L"Ultra Canvas", WS_CHILD | WS_VISIBLE, 
		0, 0, windowWidth, windowHeight, parentWindow, nullptr, programInstance, nullptr);

	if (windowHandle == nullptr)
	{
		int errorCode = GetLastError();
		Tools::ShowMessageBox(L"Window creation failed", Tools::GetErrorText(errorCode));
		exit(-1);
	}
}

Windowing::~Windowing()
{
	ShowCursor(true);

	DestroyWindow(windowHandle);
	UnregisterClass(Constants::ApplicationName.c_str(), programInstance);
}