#pragma once

#include "PrecompiledHeader.h"

#include "Tools.h"
#include "IInstanceShader.h"

using namespace std;

class ColorInstancedShader : public IInstanceShader
{
public:
	ColorInstancedShader(wstring vs, wstring ps) :IInstanceShader(vs, ps){}
	~ColorInstancedShader() {}

protected:
	virtual vector<D3D11_INPUT_ELEMENT_DESC> GetInputLayout();

private:
	ComPtr<ID3D11Buffer> matrixBuffer;
};