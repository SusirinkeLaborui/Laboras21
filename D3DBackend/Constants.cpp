#include "PrecompiledHeader.h"

#include "Constants.h"

Constants::Constants()
{
}

Constants::~Constants()
{
}

const wstring Constants::ApplicationName = L"Laboras21";
const bool Constants::VSyncEnabled = true;

const float Constants::ScreenDepth = 500000.0f;
const float Constants::ScreenNear = 0.1f;
const float Constants::FieldOfView = 50.625f;

const D3D_FEATURE_LEVEL Constants::D3DFeatureLevel = D3D_FEATURE_LEVEL_10_1;
const D3D11_CULL_MODE Constants::D3DCullMode = D3D11_CULL_BACK;
const D3D11_FILL_MODE Constants::D3DFillMode = D3D11_FILL_SOLID; // D3D11_FILL_SOLID, D3D11_FILL_WIREFRAME

const size_t Constants::NodeLimit = 20002;
const size_t Constants::EdgeLimit = Constants::NodeLimit - 1;

const float Constants::NodeSize = 20.0f;