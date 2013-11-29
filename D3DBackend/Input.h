#pragma once

#include "Tools.h"

class Input
{
private:
	bool keyMap[256];

	long mouseDeltaX, mouseDeltaY;
	float wheelDelta;
public:
	Input();
	~Input();
	
	void KeyDown(int key) { keyMap[key] = true; }
	void KeyUp(int key) { keyMap[key] = false; }
	bool IsKeyDown(int key) const { return keyMap[key]; }
	
	void SetMouseDisplacement(long x, long y);
	void SetWheelDisplacement(long delta);
	void SetWheelDisplacement(float delta);

	void HandleMouseDisplacement(long& x, long& y);
	float HandleWheelDisplacement();
};