#include "PrecompiledHeader.h"

#include "System.h"
#include "Tools.h"

System::System(int windowWidth, int windowHeight, HWND parentWindow) :
	running(false), 
	windowing(windowWidth, windowHeight, parentWindow, this),
	graphics(windowWidth, windowHeight, windowing.GetWindowHandle())
{
	RAWINPUTDEVICE Rid[2];
        
	Rid[0].usUsagePage = 0x01;				// magic numbers
	Rid[0].usUsage = 0x02;					// magically means mouse
	Rid[0].dwFlags = RIDEV_INPUTSINK;
	Rid[0].hwndTarget = windowing.GetWindowHandle();

	Rid[1].usUsagePage = 0x01;				// magic numbers
	Rid[1].usUsage = 0x06;					// magically means keyboard
	Rid[1].dwFlags = RIDEV_INPUTSINK;
	Rid[1].hwndTarget = windowing.GetWindowHandle();

	if (!RegisterRawInputDevices(Rid, 2, sizeof(Rid[0])))
	{
		Tools::ShowMessageBox(L"Error registering raw input devices", Tools::GetErrorText(GetLastError()));
		exit(1);
	}
}

System::~System()
{
}

void System::Run()
{		
	auto lastFrameFinished = secondsSinceLastFrame = Tools::GetTickCount();
	running = true;
	int frames = 0;
	float timeSinceLastFps = 0.0f;

	while (running)
	{
		//ProcessWindowsMessages();
		ProcessOneFrame();

		secondsSinceLastFrame = Tools::GetTickCount() - lastFrameFinished;
		lastFrameFinished = Tools::GetTickCount();

		frames++;
		if (lastFrameFinished - timeSinceLastFps >= 1.0f)
		{
#if _DEBUG
			OutputDebugStringW((to_wstring(frames) + L"\r\n").c_str());
#endif
			frames = 0;
			timeSinceLastFps = lastFrameFinished;
		}
	}
}

void System::RunAsync()
{
	asyncRunner = thread(&System::Run, this);
}

void System::StopRunning()
{
	running = false;
	asyncRunner.join();
}

void System::ProcessWindowsMessages()
{
	static MSG msg;

	ZeroMemory(&msg, sizeof(MSG));

	while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		DispatchMessage(&msg);
		
		if (msg.message == WM_QUIT)
		{
			running = false;
		}

		if (msg.message == WM_INPUT)
		{
			HandleRawInput(msg.lParam, msg.wParam);
		}
	}
}

void System::ProcessOneFrame()
{
	CheckInputState();

	graphics.Render(vector<InstanceData>(), vector<InstanceData>());
}

void System::CheckInputState()
{

}

long int System::MessageHandler(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam)
{
	if (message == 127)
	{
		return DefWindowProc(windowHandle, message, wParam, lParam);
	}

	switch (message)
	{
		case WM_CLOSE:
		case WM_DESTROY:
		case WM_QUIT:
			running = false;
			return 0;
			break;
		default:
			return DefWindowProc(windowHandle, message, wParam, lParam);
			break;
	}
}

void System::HandleRawInput(long lParam, long wParam)
{
	unsigned int dataSize;

	GetRawInputData((HRAWINPUT)lParam, RID_INPUT, NULL, &dataSize, sizeof(RAWINPUTHEADER));
	auto buffer = unique_ptr<unsigned char[]>(new unsigned char[dataSize]);
	GetRawInputData((HRAWINPUT)lParam, RID_INPUT, buffer.get(), &dataSize, sizeof(RAWINPUTHEADER));

	RAWINPUT* raw = (RAWINPUT*)buffer.get();

	if (raw->header.dwType == RIM_TYPEKEYBOARD) 
	{
		if (raw->data.keyboard.Message == WM_KEYDOWN && wParam == 0)
		{
			input.KeyDown(raw->data.keyboard.VKey);
		}
		else if (raw->data.keyboard.Message == WM_KEYUP)
		{
			input.KeyUp(raw->data.keyboard.VKey);
		}
	}
	else if (raw->header.dwType == RIM_TYPEMOUSE && wParam == 0) 
	{
		if (raw->data.mouse.usButtonFlags == RI_MOUSE_WHEEL)
		{
			input.SetWheelDisplacement((long)(short)raw->data.mouse.usButtonData);
		}

		input.SetMouseDisplacement(raw->data.mouse.lLastX, raw->data.mouse.lLastY);
	}
}