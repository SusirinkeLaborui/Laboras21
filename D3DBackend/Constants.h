#pragma once

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

	static const D3D_FEATURE_LEVEL D3DFeatureLevel;
	static const D3D11_CULL_MODE D3DCullMode;
	static const D3D11_FILL_MODE D3DFillMode;

	static const size_t NodeLimit;
	static const size_t EdgeLimit;
	
	static const float NodeSize;
};

