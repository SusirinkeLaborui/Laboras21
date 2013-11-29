#include "PrecompiledHeader.h"

#include "Input.h"

Input::Input() : 
	mouseDeltaX(0), mouseDeltaY(0), wheelDelta(1.0f)
{
	for_each (keyMap, keyMap + 256, [](bool& isDown)
	{
		isDown = false;
	});
}

Input::~Input()
{
}

void Input::SetMouseDisplacement(long x, long y)
{
	mouseDeltaX += x;
	mouseDeltaY += y;
}

void Input::SetWheelDisplacement(long delta)
{
	wheelDelta *= pow(2.0f, static_cast<float>(delta) / 1000.0f);
}

void Input::SetWheelDisplacement(float delta)
{
	wheelDelta *= pow(2.0f, delta / 1000.0f);
}

void Input::HandleMouseDisplacement(long& x, long& y)
{
	x = mouseDeltaX;
	y = mouseDeltaY;

	mouseDeltaX = 0;
	mouseDeltaY = 0;
}

float Input::HandleWheelDisplacement()
{
	float delta = wheelDelta;
	wheelDelta = 1.0f;

	return delta;
}