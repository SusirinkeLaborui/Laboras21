#pragma once

#include "PrecompiledHeader.h"

class Constants
{
private:
	Constants();
	~Constants();
public:
	static const wstring ApplicationName;
	static const bool VSyncEnabled;

	static const float ScreenDepth;
	static const float ScreenNear;
	static const float FieldOfView;
	static const int RenderDepth;

	static const D3D_FEATURE_LEVEL D3DFeatureLevel;
	static const D3D11_CULL_MODE D3DCullMode;
	static const D3D11_FILL_MODE D3DFillMode;

	static const bool RecalculateNormals;
	static const bool AverageNormals;
	static const bool OptimizeModel;

	static const DirectX::XMFLOAT3 FogColor;
	static const float FogPower;

	static const bool ShowCursor;
};

