#pragma once

class System;
class Windowing
{
private:
	HWND windowHandle;
	HINSTANCE programInstance;

public:
	Windowing(int windowWidth, int windowHeight, HWND parentWindow);
	~Windowing();

	HWND GetWindowHandle() { return windowHandle; }
};

