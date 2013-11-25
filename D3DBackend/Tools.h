#pragma once

#include "Constants.h"

class Tools
{
private:
	Tools();
	~Tools();

	static HWND windowHandle;
	static long long int performanceCounterFrequency;

	inline static int InitPerformanceCounterFrequency()
	{
		QueryPerformanceFrequency(reinterpret_cast<LARGE_INTEGER*>(&performanceCounterFrequency));

		return 0;
	}
public:
	static wstring GetErrorText(int errorCode);
	static void ShowMessageBox(const wstring& title, const string& text);
	static void ShowMessageBox(const wstring& title, const wstring& text);
	static void LogVector(const wstring& text, DirectX::XMVECTOR vector);

	static vector<uint8_t> ReadFileToVector(wstring path);
	static string IntToHex(int number);

	static void SetWindowHandle(HWND window) { windowHandle = window; }

	static inline DirectX::XMFLOAT4 Clamp(DirectX::XMFLOAT4 valueToClamp, float maxValueX = 1.0f, float maxValueY = 1.0f, float maxValueZ = 1.0f)
	{
		if (valueToClamp.x > maxValueX)
		{
			valueToClamp.x = maxValueX;
		}
		else if (valueToClamp.x < 0)
		{
			valueToClamp.x = 0;
		}

		if (valueToClamp.y > maxValueY)
		{
			valueToClamp.y = maxValueY;
		}
		else if (valueToClamp.y < 0)
		{
			valueToClamp.y = 0;
		}

		if (valueToClamp.z > maxValueZ)
		{
			valueToClamp.z = maxValueZ;
		}
		else if (valueToClamp.z < 0)
		{
			valueToClamp.z = 0;
		}

		return valueToClamp;
	}

	static inline float Vector3Dot(DirectX::XMVECTOR vector1, DirectX::XMVECTOR vector2)
	{
#if !WINDOWS_PHONE
		return DirectX::XMVector3Dot(vector1, vector2).m128_f32[0];
#else
		return DirectX::XMVector3Dot(vector1, vector2).n128_f32[0];
#endif
	};

	static inline float GetTickCount()
	{
		static int dummy = InitPerformanceCounterFrequency();
		long long int timer;

		QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&timer));

		return (float)timer / (float)performanceCounterFrequency;
	}
};

struct Point2D
{
	float x, y;

	Point2D() {}
	Point2D(float x, float y) : x(x), y(y) {}
};


#ifndef DEBUG
#define Assert(x)   if (x != S_OK) \
					{ \
						wstringstream stream; \
						stream << hex << x; \
						Tools::ShowMessageBox(L"Error number 0x" + stream.str(), __WFILE__ + wstring(L": ") + to_wstring(__LINE__)); \
						exit(-1); \
					}
#elif WINDOWS_PHONE
#define Assert(x)   if (x != S_OK) \
					{ \
						OutputDebugStringW((L"Error! HRESULT: " + to_wstring(x)).c_str()); \
						OutputDebugStringW((__WFILE__ + wstring(L": ") + to_wstring(__LINE__)).c_str()); \
						assert(false); \
					}
#else
#define Assert(x)   if (x != S_OK) \
					{ \
						OutputDebugStringW((L"Error! HRESULT: " + to_wstring(x)).c_str()); \
						OutputDebugStringW((__WFILE__ + wstring(L": ") + to_wstring(__LINE__)).c_str()); \
						DebugBreak(); \
					}
#endif