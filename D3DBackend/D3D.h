#pragma once

class D3D
{
private:
	int windowWidth;
	int windowHeight;

	ComPtr<ID3D11Device> device;
	ComPtr<ID3D11DeviceContext> deviceContext;
	ComPtr<ID3D11RenderTargetView> renderTargetView;
	ComPtr<ID3D11Texture2D> depthStencilBuffer;
	ComPtr<ID3D11DepthStencilState> depthStencilState;
	ComPtr<ID3D11DepthStencilView> depthStencilView;
	ComPtr<ID3D11RasterizerState> rasterState;
	ComPtr<ID3D11BlendState> alphaBlendingState;
	D3D11_VIEWPORT viewport;

	int videoCardMemory;
	string videoCardDescription;
	float aspectRatio;

	ComPtr<IDXGISwapChain> swapChain;
	HWND windowHandle;

	DirectX::XMMATRIX projectionMatrix;
	DirectX::XMMATRIX orthoMatrix;

	float currentFoV;

	void Initialize();
	void SetupRenderTargetView();
	void SetupViewport();

public:
	D3D(int windowWidth, int windowHeight, HWND windowHandle);
	~D3D();
	
	void StartDrawing(float red = 1.0f, float green = 1.0f, float blue = 1.0f, float alpha = 1.0f);
	void SwapBuffers();

	ComPtr<ID3D11Device> GetDevice() const { return device; }
	ComPtr<ID3D11DeviceContext> GetDeviceContext() const { return deviceContext; }
	ComPtr<IDXGISwapChain> GetSwapChain() const { return swapChain; }

	void ResizeContext(int newWidth, int newHeight);

	const DirectX::XMMATRIX& GetProjectionMatrix() const { return projectionMatrix; }
	const DirectX::XMMATRIX& GetOrthoMatrix() const { return orthoMatrix; }
	
	void GetVideoCardInfo(string& cardName, int& cardMemory) const { cardName = videoCardDescription; cardMemory = videoCardMemory; }
	float GetAspectRatio() const { return aspectRatio; }

	void ChangeFoV(float value);

	ComPtr<ID3D11DepthStencilView> GetDepthStencilView() const { return depthStencilView; }
	void SetBackBufferRenderTarget() { deviceContext->OMSetRenderTargets(1, renderTargetView.GetAddressOf(), depthStencilView.Get()); }
};