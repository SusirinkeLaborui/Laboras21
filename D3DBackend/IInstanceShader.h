#pragma once 

#include "IShader.h"

class IInstanceShader : public IShader
{
public:
	IInstanceShader(wstring vs, wstring ps) :IShader(vs, ps){}
	virtual ~IInstanceShader(){}

	virtual void RenderShaderInstanced(ComPtr<ID3D11DeviceContext> context, size_t indexCount, size_t instanceCount)
	{ 
		context->DrawIndexedInstanced((UINT)indexCount, (UINT)instanceCount, 0, 0, 0);
	}

	virtual void RenderShader(ComPtr<ID3D11DeviceContext> context, size_t indexCount){ AssertBool(false, L"RenderShader called on an instanced shader"); }

	virtual void SetShaderParametersInstanced(const RenderParams &params)
	{
		InstancedMatrixType matrices;
		XMStoreFloat4x4(&matrices.view, params.view);
		XMStoreFloat4x4(&matrices.projection, params.projection);
		matrices.cameraPos = params.cameraPos;

		Tools::CopyToBuffer(matrixBuffer, matrices, params.context);

		params.context->VSSetConstantBuffers(0, 1, matrixBuffer.GetAddressOf());

		params.context->IASetInputLayout(layout.Get());

		params.context->VSSetShader(vertexShader.Get(), nullptr, 0);
		params.context->PSSetShader(pixelShader.Get(), nullptr, 0);
	}

	virtual void InitializeShaderBuffers(ComPtr<ID3D11Device> device)
	{
		D3D11_BUFFER_DESC matrixBufferDesc;

		// Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
		matrixBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
		matrixBufferDesc.ByteWidth = sizeof(InstancedMatrixType);
		matrixBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
		matrixBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
		matrixBufferDesc.MiscFlags = 0;
		matrixBufferDesc.StructureByteStride = 0;

		// Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
		Assert(device->CreateBuffer(&matrixBufferDesc, nullptr, &matrixBuffer));
	}
};