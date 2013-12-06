#include "PrecompiledHeader.h"

#include "System.h"
#include "Tools.h"

System::System(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND parentWindow) :
	running(false), 
	windowing(windowWidth, windowHeight, parentWindow),
	graphics(windowWidth, windowHeight, backgroundColor, windowing.GetWindowHandle())
{
	RAWINPUTDEVICE Rid[2];
        
	Rid[0].usUsagePage = 0x01;				// magic numbers
	Rid[0].usUsage = 0x02;					// magically means mouse
	Rid[0].dwFlags = RIDEV_INPUTSINK;
	Rid[0].hwndTarget = parentWindow;

	Rid[1].usUsagePage = 0x01;				// magic numbers
	Rid[1].usUsage = 0x06;					// magically means keyboard
	Rid[1].dwFlags = RIDEV_INPUTSINK;
	Rid[1].hwndTarget = parentWindow;

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

void System::ProcessOneFrame()
{
	unique_lock<mutex> lock(drawMutex);

	CheckInputState();
	graphics.Render();
}

void System::CheckInputState()
{
	unique_lock<mutex> lock(inputMutex);
	
	auto& camera = graphics.GetCamera();
	auto cameraPos = camera.GetPosition();
	cameraPos.z = fabs(cameraPos.z);

	// Pan
	if (input.IsKeyDown('W'))
	{
		camera.Up(cameraPos.z * secondsSinceLastFrame);
	}
	if (input.IsKeyDown('S'))
	{
		camera.Up(-cameraPos.z * secondsSinceLastFrame);
	}
	if (input.IsKeyDown('A'))
	{
		camera.Right(-cameraPos.z * secondsSinceLastFrame);
	}
	if (input.IsKeyDown('D'))
	{
		camera.Right(cameraPos.z * secondsSinceLastFrame);
	}

	// Zoom
	auto wheelDisplacement = input.HandleWheelDisplacement();
	
	camera.Forward(cameraPos.z * wheelDisplacement - cameraPos.z);
}

void System::HandleRawInput(long lParam, long wParam)
{
	unsigned int dataSize;

	GetRawInputData((HRAWINPUT)lParam, RID_INPUT, nullptr, &dataSize, sizeof(RAWINPUTHEADER));
	auto buffer = unique_ptr<unsigned char[]>(new unsigned char[dataSize]);
	GetRawInputData((HRAWINPUT)lParam, RID_INPUT, buffer.get(), &dataSize, sizeof(RAWINPUTHEADER));

	RAWINPUT* raw = (RAWINPUT*)buffer.get();
	unique_lock<mutex> lock(inputMutex);

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

void System::ResizeWindow(int newWidth, int newHeight)
{
	unique_lock<mutex> lock(drawMutex);

	MoveWindow(windowing.GetWindowHandle(), 0, 0, newWidth, newHeight, TRUE);
	graphics.ResizeD3DContext(newWidth, newHeight);
}

void System::DrawNodes(const Point* nodeList, int nodeCount)
{
	unique_lock<mutex> lock(drawMutex);
	graphics.GetNodes().Add(nodeList, nodeCount);
}

void System::DrawEdge(const Point& nodeA, const Point& nodeB)
{
	unique_lock<mutex> lock(drawMutex);
	graphics.GetEdges().Add(nodeA, nodeB);
}

void System::DrawEdges(pair<Point, Point>* nodes, int count)
{
	unique_lock<mutex> lock(drawMutex);
	graphics.GetEdges().AddBatch(nodes, count);
}

void System::ClearNodes()
{
	unique_lock<mutex> lock(drawMutex);
	graphics.GetNodes().Clear();
}

void System::ClearEdges()
{
	unique_lock<mutex> lock(drawMutex);
	graphics.GetEdges().Clear();
}

void* System::operator new(size_t size)
{
	void* ptr = _aligned_malloc(size, 16);

	if (ptr == nullptr)
	{
		throw bad_alloc();
	}

	return ptr;
}

void System::operator delete(void* p)
{
	auto ptr = static_cast<System*>(p);
	_aligned_free(p);
}