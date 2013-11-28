#pragma once

#include "Constants.h"

typedef void (__stdcall *MessageBoxCallback)(const wchar_t* title, const wchar_t* text);

class Tools
{
private:
	Tools();
	~Tools();

	static MessageBoxCallback messageBoxCallback;
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

	static vector<uint8_t> ReadFileToVector(wstring path);
	static string IntToHex(int number);

	static void SetMessageBoxCallback(MessageBoxCallback callback) { messageBoxCallback = callback;	}


	template<class T>
	static void CopyToBuffer(Microsoft::WRL::ComPtr<ID3D11Buffer> buffer, const T &data, Microsoft::WRL::ComPtr<ID3D11DeviceContext> context)
	{
		D3D11_MAPPED_SUBRESOURCE resource;
		context->Map(buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &resource);
		memcpy(resource.pData, &data, sizeof(T));
		context->Unmap(buffer.Get(), 0);
	}

	template<class T>
	static void Reverse(std::vector<T> &vec)
	{
		reverse(vec.begin(), vec.end());
	}

	static bool ReadFileToArray(wstring file, std::unique_ptr<char> &arr, UINT &size);

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


#ifndef _DEBUG
#define Assert(x) \
if (x != S_OK) \
{ \
	wstringstream stream; \
	stream << hex << x; \
	Tools::ShowMessageBox(L"Error number 0x" + stream.str(), __WFILE__ + wstring(L": ") + to_wstring(__LINE__)); \
	exit(-1); \
}
#else
#define Assert(x) \
if (x != S_OK) \
{ \
	wstringstream stream; \
	stream << hex << x; \
	Tools::ShowMessageBox(L"Error number 0x" + stream.str(), __WFILE__ + wstring(L": ") + to_wstring(__LINE__)); \
	DebugBreak(); \
}
#endif

#ifndef _DEBUG
#define AssertBool(x, error) \
if (x != true) \
{ \
	Tools::ShowMessageBox(error, __WFILE__ + wstring(L": ") + to_wstring(__LINE__)); \
	exit(-1); \
}
#else
#define AssertBool(x, error) \
if (x != true) \
{ \
	Tools::ShowMessageBox(error, __WFILE__ + wstring(L": ") + to_wstring(__LINE__)); \
	DebugBreak(); \
}
#endif