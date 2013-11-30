#include "PrecompiledHeader.h"
#include "Camera.h"
using namespace DirectX;

Camera::Camera() : up(0.0f, 1.0f, 0.0f), pos(0.0f, 0.0f, 0.0f), forward(0.0f, 0.0f, 1.0f), right(1.0f, 0.0f, 0.0f), modified(true)
{
}

void Camera::RenderMain()
{
	if (modified)
	{
		modified = false;
		XMStoreFloat4x4(&viewMatrix, XMMatrixLookToLH(XMLoadFloat3(&pos), XMLoadFloat3(&forward), XMLoadFloat3(&up)));
	}
}

XMMATRIX Camera::GetReflectedViewMatrix(const XMMATRIX &reflect, const XMMATRIX &zeroReflect)
{
	XMVECTOR up = XMVector3Transform(XMLoadFloat3(&this->up), zeroReflect);
	XMVECTOR forward = XMVector3Transform(XMLoadFloat3(&this->forward), zeroReflect);
	XMVECTOR pos = XMVector3Transform(XMLoadFloat3(&this->pos), reflect);
	return XMMatrixLookToLH(pos, forward, up);
}

void Camera::Yaw(float angle)
{
	XMMATRIX matrix = XMMatrixRotationAxis(XMLoadFloat3(&up), angle);

	XMStoreFloat3(&right, XMVector3Transform(XMLoadFloat3(&right), matrix));
	XMStoreFloat3(&forward, XMVector3Transform(XMLoadFloat3(&forward), matrix));
	modified = true;
}

void Camera::Pitch(float angle)
{
	XMMATRIX matrix = XMMatrixRotationAxis(XMLoadFloat3(&right), angle);

	XMStoreFloat3(&up, XMVector3Transform(XMLoadFloat3(&up), matrix));
	XMStoreFloat3(&forward, XMVector3Transform(XMLoadFloat3(&forward), matrix));
	modified = true;
}

void Camera::Roll(float angle)
{
	XMMATRIX matrix = XMMatrixRotationAxis(XMLoadFloat3(&forward), angle);

	XMStoreFloat3(&right, XMVector3Transform(XMLoadFloat3(&right), matrix));
	XMStoreFloat3(&up, XMVector3Transform(XMLoadFloat3(&up), matrix));
	modified = true;
}

void Camera::Up(float dist)
{
	XMVECTOR pos = XMLoadFloat3(&this->pos);
	XMVECTOR up = XMLoadFloat3(&this->up);
	XMStoreFloat3(&this->pos, pos + dist * up);
	modified = true;
}

void Camera::Right(float dist)
{
	XMVECTOR pos = XMLoadFloat3(&this->pos);
	XMVECTOR right = XMLoadFloat3(&this->right);
	XMStoreFloat3(&this->pos, pos + dist * right);
	modified = true;
}

void Camera::Forward(float dist)
{
	XMVECTOR pos = XMLoadFloat3(&this->pos);
	XMVECTOR forward = XMLoadFloat3(&this->forward);
	XMStoreFloat3(&this->pos, pos + dist * forward);
	modified = true;
}

void Camera::Move(float x, float y, float z)
{
	Forward(z);
	Right(x);
	Up(y);
}