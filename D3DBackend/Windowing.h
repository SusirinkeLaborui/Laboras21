#pragma once

class System;
class Windowing
{
private:
	HWND windowHandle;
	HINSTANCE programInstance;

public:
	Windowing(int windowWidth, int windowHeight, HWND parentWindow, System* systemInstance);
	~Windowing();

	HWND GetWindowHandle() { return windowHandle; }
};

