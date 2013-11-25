#include "PrecompiledHeader.h"

#include "Tools.h"

HWND Tools::windowHandle = nullptr;
long long int Tools::performanceCounterFrequency = 0;

Tools::Tools()
{
}

Tools::~Tools()
{
}

wstring Tools::GetErrorText(int errorCode)
{
	wchar_t buffer[256];

	::FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,	NULL, errorCode, MAKELANGID(LANG_NEUTRAL,SUBLANG_DEFAULT), buffer, 255,	NULL);

	return buffer;
}

void Tools::ShowMessageBox(const wstring& title, const string& text)
{
#if !WINDOWS_PHONE
	auto buffer = unique_ptr<wchar_t[]>(new wchar_t[text.length() + 1]);
	MultiByteToWideChar(CP_THREAD_ACP, 0, text.c_str(), text.length(), buffer.get(), text.length());
	buffer[text.length()] = 0;
	ShowMessageBox(title, buffer.get());
#endif
}

void Tools::ShowMessageBox(const wstring& title, const wstring& text)
{
#if !WINDOWS_PHONE
	MessageBox(windowHandle, text.c_str(), title.c_str(), MB_OK);
#endif
}

void Tools::LogVector(const wstring& text, DirectX::XMVECTOR vector)
{
	wstring vectorStr;
#if !WINDOWS_PHONE
	vectorStr = L"[" + to_wstring(vector.m128_f32[0]) + L"; " + to_wstring(vector.m128_f32[1]) + L"; " + 
		to_wstring(vector.m128_f32[2]) + L"; " + to_wstring(vector.m128_f32[3]) + L"]";
#else
	vectorStr = L"[" + to_wstring(vector.n128_f32[0]) + L"; " + to_wstring(vector.n128_f32[1]) + L"; " + 
		to_wstring(vector.n128_f32[2]) + L"; " + to_wstring(vector.n128_f32[3]) + L"]";
#endif

	OutputDebugString((text + vectorStr + L"\r\n").c_str());
}

vector<uint8_t> Tools::ReadFileToVector(wstring path)
{
	ifstream in(path, ios::binary);

	if (!in.is_open())
	{
		ShowMessageBox(L"Error!", L"Could not open file " + path + L".");
		exit(-1);
	}
	
	in.seekg(0, ios::end);
	int fileLength = static_cast<int>(in.tellg());
	vector<uint8_t> fileContents(fileLength);
	in.seekg(0, ios::beg);

	in.read(reinterpret_cast<char*>(&fileContents[0]), fileLength);
	in.close();

	return fileContents;	
}

string Tools::IntToHex(int number)	
{
	string str;

	for (short int i = 0; i < 4; i++)
	{
		str += char(number % 256);
		number /= 256;
	}
	return str;
}