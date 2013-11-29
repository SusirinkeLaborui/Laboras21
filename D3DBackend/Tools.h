#pragma once

#include "Constants.h"

typedef void (*MessageBoxCallback)(const wchar_t* title, const wchar_t* text);

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