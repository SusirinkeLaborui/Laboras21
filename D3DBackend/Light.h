#pragma once

class Light
{
private:
	DirectX::XMFLOAT3 direction;
	DirectX::XMFLOAT4 directionalColor;
	DirectX::XMFLOAT4 ambientColor;
	
	void Normalize();
public:
	Light(DirectX::XMFLOAT3 direction, DirectX::XMFLOAT4 directionalColor, DirectX::XMFLOAT4 ambientColor);
	~Light();
	
	const DirectX::XMFLOAT3& GetDirection() const { return direction; }
	const DirectX::XMFLOAT4& GetDirectionalColor() const { return directionalColor; }
	const DirectX::XMFLOAT4& GetAmbientColor() const { return ambientColor; }

	void SetDirection(const DirectX::XMFLOAT3& dir) { direction = dir; Normalize(); }
	void SetDirectionalColor(const DirectX::XMFLOAT4& col) { directionalColor = col; }
	void SetAmbientColor(const DirectX::XMFLOAT4& col) { ambientColor = col; }
};

