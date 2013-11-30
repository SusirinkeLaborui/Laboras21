#include "PrecompiledHeader.h"

#include "D3D.h"
#include "Tools.h"
#include "Constants.h"

D3D::D3D(int windowWidth, int windowHeight, HWND windowHandle) :
	windowHandle(windowHandle),
	videoCardMemory(0),
	windowWidth(windowWidth), 
	windowHeight(windowHeight)
{
	Initialize();
}

D3D::~D3D()
{
}

void D3D::Initialize()
{
	HRESULT result;
	unsigned int numerator = 0,
				 denominator = 1;
	ComPtr<ID3D11Texture2D> backBufferPtr;
	ComPtr<IDXGIAdapter> adapter;
	D3D11_TEXTURE2D_DESC depthBufferDesc;
	D3D11_DEPTH_STENCIL_DESC depthStencilDesc;
	D3D11_DEPTH_STENCIL_DESC disabledDepthStencilDesc;
	D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;
	D3D11_RASTERIZER_DESC rasterDesc;
	D3D11_BLEND_DESC blendStateDescription;

	DXGI_SWAP_CHAIN_DESC swapChainDesc;
	ComPtr<IDXGIFactory> factory;

	// Create a DirectX graphics interface factory.
	result = CreateDXGIFactory(__uuidof(IDXGIFactory), (void**)&factory);
	Assert(result);

	// Use the factory to create an adapter for the primary graphics interface (video card).
	result = factory->EnumAdapters(0, &adapter);
	if (result == S_OK)
	{
		ComPtr<IDXGIOutput> adapterOutput;
		unsigned int numModes, stringLength;
		unique_ptr<DXGI_MODE_DESC[]> displayModeList;
		DXGI_ADAPTER_DESC adapterDesc;

		// Enumerate the primary adapter output (monitor).
		result = adapter->EnumOutputs(0, &adapterOutput);
		Assert(result);
	
		// Get the number of modes that fit the DXGI_FORMAT_R8G8B8A8_UNORM display format for the adapter output (monitor).
		result = adapterOutput->GetDisplayModeList(DXGI_FORMAT_R8G8B8A8_UNORM, DXGI_ENUM_MODES_INTERLACED, &numModes, NULL);
		if (result == S_OK)
		{
			// Create a list to hold all the possible display modes for this monitor/video card combination.
			displayModeList = unique_ptr<DXGI_MODE_DESC[]>(new DXGI_MODE_DESC[numModes]);

			// Now fill the display mode list structures.
			result = adapterOutput->GetDisplayModeList(DXGI_FORMAT_R8G8B8A8_UNORM, DXGI_ENUM_MODES_INTERLACED, &numModes, displayModeList.get());
		
			if (result == S_OK)
			{
				// Fallback value
				numerator = displayModeList[0].RefreshRate.Numerator;
				denominator = displayModeList[0].RefreshRate.Denominator;

				// Now go through all the display modes and find the one that matches the screen width and height.
				// When a match is found store the numerator and denominator of the refresh rate for that monitor.
				for_each (displayModeList.get(), displayModeList.get() + numModes, [&numerator, &denominator, this](const DXGI_MODE_DESC& mode)
				{
					if (mode.Width == windowWidth)
					{
						if (mode.Height == windowHeight)
						{
							numerator = mode.RefreshRate.Numerator;
							denominator = mode.RefreshRate.Denominator;
						}
					}
				});

				// Get the adapter (video card) description.
				result = adapter->GetDesc(&adapterDesc);
				Assert(result);

				// Store the dedicated video card memory in megabytes.
				videoCardMemory = adapterDesc.DedicatedVideoMemory / (1024 * 1024);

				// Convert the name of the video card to a character array and store it.
				char buffer[128];
				result = wcstombs_s(&stringLength, buffer, 128, adapterDesc.Description, 128);
				Assert(result);

				videoCardDescription = buffer;
			}
			else
			{
				Tools::ShowMessageBox(L"Error number " + to_wstring(result), L"Could not get list of available display modes.");
			}
		}
		else
		{
			Tools::ShowMessageBox(L"Warning", L"Unable to get display mode list");
		}
	}
	else
	{
		Tools::ShowMessageBox(L"Error number " + to_wstring(result), L"Could not get GPU description.");
	}

	// Initialize the swap chain description.
	ZeroMemory(&swapChainDesc, sizeof(swapChainDesc));

	// Set the width and height of the back buffer.
	swapChainDesc.BufferDesc.Width = windowWidth;
	swapChainDesc.BufferDesc.Height = windowHeight;

	// Set regular 32-bit surface for the back buffer.
	swapChainDesc.BufferDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;

	// Set the refresh rate of the back buffer.
	if (Constants::VSyncEnabled)
	{
		swapChainDesc.BufferDesc.RefreshRate.Numerator = numerator;
		swapChainDesc.BufferDesc.RefreshRate.Denominator = denominator;
	}
	else
	{
		swapChainDesc.BufferDesc.RefreshRate.Numerator = 0;
		swapChainDesc.BufferDesc.RefreshRate.Denominator = 1;
	}

	// Set the handle for the window to render to.
	swapChainDesc.OutputWindow = windowHandle;

	// Turn multisampling off.
	swapChainDesc.SampleDesc.Count = 1;
	swapChainDesc.SampleDesc.Quality = 0;

	swapChainDesc.Windowed = true;

	// Set the scan line ordering and scaling to unspecified.
	swapChainDesc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
	swapChainDesc.BufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;

	swapChainDesc.OutputWindow = windowHandle;
	
	// Set to a single back buffer.
	swapChainDesc.BufferCount = 1;

	// Discard the back buffer contents after presenting.
	swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	// Set the usage of the back buffer.
	swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;

	swapChainDesc.Flags = 0;

	D3D_FEATURE_LEVEL featureLevel = Constants::D3DFeatureLevel;
	auto deviceFlags = 0u;
	
#if _DEBUG
	deviceFlags |= D3D11_CREATE_DEVICE_DEBUG;		// Fails on Windows 8.1 if 8.1 SDK is not installed
#endif

	// Create the swap chain, Direct3D device, and Direct3D device context.
	result = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, deviceFlags, &featureLevel, 1, 
					       D3D11_SDK_VERSION, &swapChainDesc, &swapChain, &device, nullptr, &deviceContext);
	Assert(result);

	// Get the pointer to the back buffer.
	result = swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), &backBufferPtr);
	Assert(result);

	// Create the render target view with the back buffer pointer.
	result = device->CreateRenderTargetView(backBufferPtr.Get(), NULL, &renderTargetView);
	Assert(result);

	// Initialize the description of the depth buffer.
	ZeroMemory(&depthBufferDesc, sizeof(depthBufferDesc));

	// Set up the description of the depth buffer.
	depthBufferDesc.Width = windowWidth;
	depthBufferDesc.Height = windowHeight;

	depthBufferDesc.MipLevels = 1;
	depthBufferDesc.ArraySize = 1;
	depthBufferDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
	depthBufferDesc.SampleDesc.Count = 1;
	depthBufferDesc.SampleDesc.Quality = 0;
	depthBufferDesc.Usage = D3D11_USAGE_DEFAULT;
	depthBufferDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
	depthBufferDesc.CPUAccessFlags = 0;
	depthBufferDesc.MiscFlags = 0;

	// Create the texture for the depth buffer using the filled out description.
	result = device->CreateTexture2D(&depthBufferDesc, NULL, &depthStencilBuffer);
	Assert(result);

	// Initialize the description of the stencil state.
	ZeroMemory(&depthStencilDesc, sizeof(depthStencilDesc));

	// Set up the description of the stencil state.
	depthStencilDesc.DepthEnable = true;
	depthStencilDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
	depthStencilDesc.DepthFunc = D3D11_COMPARISON_LESS;

	depthStencilDesc.StencilEnable = true;
	depthStencilDesc.StencilReadMask = 0xFF;
	depthStencilDesc.StencilWriteMask = 0xFF;

	// Stencil operations if pixel is front-facing.
	depthStencilDesc.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
	depthStencilDesc.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_INCR;
	depthStencilDesc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
	depthStencilDesc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

	// Stencil operations if pixel is back-facing.
	depthStencilDesc.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
	depthStencilDesc.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_DECR;
	depthStencilDesc.BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
	depthStencilDesc.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

	// Create the depth stencil state.
	result = device->CreateDepthStencilState(&depthStencilDesc, &depthStencilState);
	Assert(result);

	ZeroMemory(&disabledDepthStencilDesc, sizeof(disabledDepthStencilDesc));

	// Now create a second depth stencil state which turns off the Z buffer for 2D rendering.  The only difference is 
	// that DepthEnable is set to false, all other parameters are the same as the other depth stencil state.
	disabledDepthStencilDesc.DepthEnable = false;
	disabledDepthStencilDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
	disabledDepthStencilDesc.DepthFunc = D3D11_COMPARISON_LESS;

	disabledDepthStencilDesc.StencilEnable = true;
	disabledDepthStencilDesc.StencilReadMask = 0xFF;
	disabledDepthStencilDesc.StencilWriteMask = 0xFF;

	disabledDepthStencilDesc.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
	disabledDepthStencilDesc.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_INCR;
	disabledDepthStencilDesc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
	disabledDepthStencilDesc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

	disabledDepthStencilDesc.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
	disabledDepthStencilDesc.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_DECR;
	disabledDepthStencilDesc.BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
	disabledDepthStencilDesc.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

	// Create the state using the device.
	result = device->CreateDepthStencilState(&disabledDepthStencilDesc, &disabledDepthStencilState);
	Assert(result);

	// Set the depth stencil state.
	deviceContext->OMSetDepthStencilState(depthStencilState.Get(), 1);

	// Initailze the depth stencil view.
	ZeroMemory(&depthStencilViewDesc, sizeof(depthStencilViewDesc));

	// Set up the depth stencil view description.
	depthStencilViewDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
	depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
	depthStencilViewDesc.Texture2D.MipSlice = 0;

	// Create the depth stencil view.
	result = device->CreateDepthStencilView(depthStencilBuffer.Get(), &depthStencilViewDesc, &depthStencilView);
	Assert(result);

	// Bind the render target view and depth stencil buffer to the output render pipeline.
	deviceContext->OMSetRenderTargets(1, renderTargetView.GetAddressOf(), depthStencilView.Get());

	// Setup the raster description which will determine how and what polygons will be drawn.
	rasterDesc.FillMode = Constants::D3DFillMode;
	rasterDesc.CullMode = Constants::D3DCullMode;
	rasterDesc.FrontCounterClockwise = false;
	rasterDesc.DepthBias = 0;
	rasterDesc.DepthBiasClamp = 0.0f;
	rasterDesc.SlopeScaledDepthBias = 0.0f;
	rasterDesc.DepthClipEnable = true;
	rasterDesc.ScissorEnable = false;
	rasterDesc.MultisampleEnable = true;
	rasterDesc.AntialiasedLineEnable = false;

	// Create the rasterizer state from the description we just filled out.
	result = device->CreateRasterizerState(&rasterDesc, &rasterState);
	Assert(result);

	// Now set the rasterizer state.
	deviceContext->RSSetState(rasterState.Get());

	// Clear the blend state description.
	ZeroMemory(&blendStateDescription, sizeof(D3D11_BLEND_DESC));
	
	blendStateDescription.AlphaToCoverageEnable = false;
	
	// Create an alpha enabled blend state description.
	blendStateDescription.RenderTarget[0].BlendEnable = true;
	blendStateDescription.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
	blendStateDescription.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	blendStateDescription.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
	blendStateDescription.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
	blendStateDescription.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
	blendStateDescription.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
	blendStateDescription.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
	
	// Create the blend state using the description.
	result = device->CreateBlendState(&blendStateDescription, &alphaBlendingState);
	Assert(result);

	float blendFactor[4];
	
	// Setup the blend factor.
	blendFactor[0] = 0.0f;
	blendFactor[1] = 0.0f;
	blendFactor[2] = 0.0f;
	blendFactor[3] = 0.0f;

	// Turn on the alpha blending.
	deviceContext->OMSetBlendState(alphaBlendingState.Get(), blendFactor, 0xffffffff);

	SetupViewport();
}

void D3D::SetupViewport()
{
	viewport.Width = static_cast<float>(windowWidth);
	viewport.Height = static_cast<float>(windowHeight);
	viewport.MinDepth = 0.0f;
	viewport.MaxDepth = 1.0f;
	viewport.TopLeftX = 0.0f;
	viewport.TopLeftY = 0.0f;

	aspectRatio = static_cast<float>(windowWidth) / static_cast<float>(windowHeight);
	currentFoV = DirectX::XM_PI * Constants::FieldOfView / 180.0f;

	projectionMatrix = DirectX::XMMatrixPerspectiveFovRH(currentFoV, aspectRatio, Constants::ScreenNear, Constants::ScreenDepth);
	orthoMatrix = DirectX::XMMatrixOrthographicRH(static_cast<float>(windowWidth), static_cast<float>(windowHeight), 
		Constants::ScreenNear, Constants::ScreenDepth);
}

void D3D::ResizeContext(int newWidth, int newHeight)
{
	HRESULT result;
	ComPtr<ID3D11Texture2D> backBufferPtr;

	// Release all references to current backbuffer renderTargetView, as it will get recreated.
	deviceContext->OMSetRenderTargets(0, nullptr, nullptr);
	renderTargetView = nullptr;

    result = swapChain->ResizeBuffers(1, newWidth, newHeight, DXGI_FORMAT_R8G8B8A8_UNORM, 0);
    Assert(result);

	// Recreate back buffer renderTarget
	result = swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), &backBufferPtr);
	Assert(result);

	result = device->CreateRenderTargetView(backBufferPtr.Get(), NULL, &renderTargetView);
	Assert(result);

	windowWidth = newWidth;
	windowHeight = newHeight;
	
	SetBackBufferRenderTarget();
	SetupViewport();
}

void D3D::StartDrawing(float red, float green, float blue, float alpha)
{
	float color[] = { red, green, blue, alpha };

	// Set the viewport.
	deviceContext->RSSetViewports(1, &viewport);

	// Clear the back buffer.
	deviceContext->ClearRenderTargetView(renderTargetView.Get(), color);
    
	// Clear the depth buffer.
	deviceContext->ClearDepthStencilView(depthStencilView.Get(), D3D11_CLEAR_DEPTH, 1.0f, 0);
}

void D3D::SwapBuffers()
{	
	swapChain->Present(1, 0);
}

void D3D::ChangeFoV(float value)
{
	currentFoV += value;
	projectionMatrix = DirectX::XMMatrixPerspectiveFovRH(currentFoV, (float)windowWidth / (float)windowHeight, Constants::ScreenNear, Constants::ScreenDepth);
}
