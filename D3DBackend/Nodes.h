#pragma once

#include "BaseInstancer.h"

class Nodes : public Instancer
{
public:
	Nodes(ColorModel &model, ColorInstancedShader &shader, size_t maxObjectCount):BaseInstancer(model, shader, maxObjectCount){}
	
	void Add(Point point);
	void Add(const Point *points, size_t count);
private:
	static pair<XMFLOAT4X4, XMFLOAT4X4> GetNodeMatrix(Point p);
};