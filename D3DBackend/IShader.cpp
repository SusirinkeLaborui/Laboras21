#include "PrecompiledHeader.h"
#include "IShader.h"
#include "Tools.h"

using namespace std;

void IShader::Init(ComPtr<ID3D11Device> device)
{
	InitializeShader(device, vs, ps, GetInputLayout());
	InitializeShaderBuffers(device);
}

void IShader::InitializeShader(ComPtr<ID3D11Device> device, wstring vs, wstring ps, const vector<D3D11_INPUT_ELEMENT_DESC> &inputLayout)
{
	unique_ptr<char> vBuffer;
	UINT vSize;

	AssertBool(Tools::ReadFileToArray(vs, vBuffer, vSize), L"Could not read " + vs);

	// Create the vertex shader from the buffer.
	Assert(device->CreateVertexShader(vBuffer.get(), vSize, nullptr, &vertexShader));

	unique_ptr<char> pBuffer;
	UINT pSize;
	AssertBool(Tools::ReadFileToArray(ps, pBuffer, pSize), L"Could not read " + ps);

	// Create the pixel shader from the buffer.
	Assert(device->CreatePixelShader(pBuffer.get(), pSize, nullptr, &pixelShader));

	Assert(device->CreateInputLayout(&inputLayout[0], static_cast<UINT>(inputLayout.size()), vBuffer.get(), vSize, &layout));
}