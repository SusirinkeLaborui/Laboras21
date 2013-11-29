#include "PrecompiledHeader.h"
#include "Windowing.h"
#include "System.h"


static long int CALLBACK HandleMessage(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam);
static System* SystemInstance;

Windowing::Windowing(int windowWidth, int windowHeight, HWND parentWindow, System* systemInstance)
{
	WNDCLASSEX windowInfo;
	int posX, posY;

	SystemInstance = systemInstance;

	auto programInstance = GetModuleHandle(NULL);

	ZeroMemory(&windowInfo, sizeof(WNDCLASSEX));
	windowInfo.style = CS_HREDRAW | CS_VREDRAW | CS_PARENTDC;
	windowInfo.lpfnWndProc = HandleMessage;
	windowInfo.hInstance = programInstance;
	windowInfo.hIcon = LoadIcon(NULL, IDI_WINLOGO);
	windowInfo.hIconSm = windowInfo.hIcon;
	windowInfo.hCursor = LoadCursor(NULL, IDC_ARROW);
	windowInfo.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
	windowInfo.lpszMenuName = NULL;
	windowInfo.lpszClassName = Constants::ApplicationName.c_str();
	windowInfo.cbSize = sizeof(WNDCLASSEX);
	
	// Register the window class.
	RegisterClassEx(&windowInfo);
	
	posX = (GetSystemMetrics(SM_CXSCREEN) - windowWidth) / 2;
	posY = (GetSystemMetrics(SM_CYSCREEN) - windowHeight) / 2;

	windowHandle = CreateWindowEx(0, windowInfo.lpszClassName, L"Ultra Canvas", WS_CHILD, 
		0, 0, windowWidth, windowHeight, parentWindow, nullptr, programInstance, nullptr);

	if (windowHandle == nullptr)
	{
		int errorCode = GetLastError();
		Tools::ShowMessageBox(L"Window creation failed", Tools::GetErrorText(errorCode));
		exit(-1);
	}

	// Bring the window up on the screen and set it as main focus.
	ShowWindow(windowHandle, SW_SHOW);
	//SetForegroundWindow(windowHandle);
	//SetFocus(windowHandle);

	ShowCursor(Constants::ShowCursor);
}

Windowing::~Windowing()
{
	ShowCursor(true);

	DestroyWindow(windowHandle);
	UnregisterClass(Constants::ApplicationName.c_str(), programInstance);
}

long int CALLBACK HandleMessage(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam)
{
	return SystemInstance->MessageHandler(windowHandle, message, wParam, lParam);
}