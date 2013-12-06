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
	void AppendMatrix(D3D11_INPUT_ELEMENT_DESC& matrixDesc, vector<D3D11_INPUT_ELEMENT_DESC>& destination);

private:
	ComPtr<ID3D11Buffer> matrixBuffer;
};