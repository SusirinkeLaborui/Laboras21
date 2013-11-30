#include "PrecompiledHeader.h"
#include "Light.h"


Light::Light(DirectX::XMFLOAT3 direction, DirectX::XMFLOAT4 directionalColor, DirectX::XMFLOAT4 ambientColor) :
	direction(direction), directionalColor(directionalColor), ambientColor(ambientColor)
{
}


Light::~Light()
{
}

void Light::Normalize()
{
	float length = sqrt(direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
	direction.x /= length;
	direction.y /= length;
	direction.z /= length;
}