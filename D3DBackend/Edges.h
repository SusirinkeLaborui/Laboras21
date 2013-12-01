#pragma once
#include "BaseInstancer.h"

class Edges : public Instancer
{
public:
	Edges(ColorModel &model, ColorInstancedShader &shader, size_t maxObjectCount) :BaseInstancer(model, shader, maxObjectCount){}
	void Add(Point point);
	void Add(const Point *points, size_t count);
private:
	static XMFLOAT4X4 GetEdgeMatrix(Point p);
};

