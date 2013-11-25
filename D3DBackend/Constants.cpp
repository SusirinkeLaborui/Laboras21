#include "PrecompiledHeader.h"

#include "Constants.h"

Constants::Constants(void)
{
}

Constants::~Constants(void)
{
}

const wstring Constants::ApplicationName = L"DirectX test";
#if !WINDOWS_PHONE
const bool Constants::VSyncEnabled = false;
#else
const bool Constants::VSyncEnabled = true;	
#endif

const float Constants::ScreenDepth = 5000.0f;
const float Constants::ScreenNear = 0.1f;
const float Constants::FieldOfView = 50.625f;
const int Constants::RenderDepth = 0;

const D3D_FEATURE_LEVEL Constants::D3DFeatureLevel = D3D_FEATURE_LEVEL_10_1;
const D3D11_CULL_MODE Constants::D3DCullMode = D3D11_CULL_BACK;
const D3D11_FILL_MODE Constants::D3DFillMode = D3D11_FILL_SOLID; // D3D11_FILL_SOLID, D3D11_FILL_WIREFRAME

const bool Constants::RecalculateNormals = true;
const bool Constants::AverageNormals = true;
const bool Constants::OptimizeModel = true;

const DirectX::XMFLOAT3 Constants::FogColor = DirectX::XMFLOAT3(0.5f, 0.5f, 0.5f);
const float Constants::FogPower = 0.0f;

const bool Constants::ShowCursor = false;