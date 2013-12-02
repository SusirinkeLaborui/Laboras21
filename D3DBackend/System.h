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

	void ProcessOneFrame();
	void CheckInputState();

	thread asyncRunner;
	mutex drawMutex;
	mutex inputMutex;

	void Run();
public:
	System(int windowWidth, int windowHeight, DirectX::XMFLOAT4 backgroundColor, HWND parentWindow);
	~System();

	void HandleRawInput(long lParam, long wParam);

	void RunAsync();
	void StopRunning();

	Input& GetInputObject() { return input; }	
	
	void ResizeWindow(int newWidth, int newHeight);
	
    void DrawNodes(const Point* nodeList, int nodeCount);
    void DrawEdge(const Point& nodeA, const Point& nodeB);
	void DrawEdges(pair<Point, Point>* nodes, int count);
	void ClearNodes();
	void ClearEdges();

	void* operator new(size_t size);
	void operator delete(void* p);
};
