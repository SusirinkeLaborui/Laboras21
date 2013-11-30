#pragma once

#include "Globals.h"
#include "Tools.h"

using namespace Microsoft::WRL;
using namespace std;
using namespace DirectX;

class IShader
{
public:
	IShader(wstring vs, wstring ps){ this->vs = vs; this->ps = ps; }
	IShader(IShader&) = delete;
	IShader &operator=(IShader&) = delete;
	virtual ~IShader(){}
	virtual void Init(ComPtr<ID3D11Device> device);
	virtual void RenderShader(ComPtr<ID3D11DeviceContext> context, size_t indexCount){context->DrawIndexed((UINT)indexCount, 0, 0);}
protected:
	void InitializeShader(ComPtr<ID3D11Device> device, wstring vs, wstring ps, const vector<D3D11_INPUT_ELEMENT_DESC> &inputLayout);
	virtual void InitializeShaderBuffers(ComPtr<ID3D11Device> device) = 0;
	virtual vector<D3D11_INPUT_ELEMENT_DESC> GetInputLayout() = 0;

	ComPtr<ID3D11VertexShader> vertexShader;
	ComPtr<ID3D11PixelShader> pixelShader;
	ComPtr<ID3D11InputLayout> layout;

	ComPtr<ID3D11Buffer> matrixBuffer;
private:
	wstring vs, ps;
};