#pragma once

#include "Input.h"
#include "Graphics.h"
#include "Windowing.h"

class System
{
private:
	Windowing windowing;
	Input input;
	Graphics graphics;

	bool running;
	float secondsSinceLastFrame;

	void ProcessWindowsMessages();
	void ProcessOneFrame();
	void CheckInputState();
	void HandleRawInput(long lParam, long wParam);

	thread asyncRunner;

	void Run();
public:
	System(int windowWidth, int windowHeight, HWND parentWindow);
	~System();

	long int MessageHandler(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam);

	void RunAsync();
	void StopRunning();

	Input& GetInputObject() { return input; }
};
