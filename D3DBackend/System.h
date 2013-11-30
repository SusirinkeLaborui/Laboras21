#pragma once

#include "Input.h"
#include "Graphics.h"
#include "Windowing.h"
#include "ResourceManager.h"

class System
{
private:
	Windowing windowing;
	ResourceManager manager;
	Input input;
	Graphics graphics;

	bool running;
	float secondsSinceLastFrame;

	void ProcessWindowsMessages();
	void ProcessOneFrame();
	void CheckInputState();
	void HandleRawInput(long lParam, long wParam);

	thread asyncRunner;
	mutex drawMutex;

	void Run();
public:
	System(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND parentWindow);
	~System();

	long int MessageHandler(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam);

	void RunAsync();
	void StopRunning();

	Input& GetInputObject() { return input; }	
	
	void ResizeWindow(int newWidth, int newHeight);
	
    void DrawNodes(const Point* nodeList, int nodeCount);
    void DrawEdge(const Point& nodeA, const Point& nodeB);
	void ClearNodes();
	void ClearEdges();

	void* operator new(size_t size);
	void operator delete(void* p);
};
