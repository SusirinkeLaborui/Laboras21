#include "PrecompiledHeader.h"

#include "Tools.h"

MessageBoxCallback Tools::messageBoxCallback = nullptr;
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
	auto buffer = unique_ptr<wchar_t[]>(new wchar_t[text.length() + 1]);
	MultiByteToWideChar(CP_THREAD_ACP, 0, text.c_str(), text.length(), buffer.get(), text.length());
	buffer[text.length()] = 0;
	ShowMessageBox(title, buffer.get());
}

void Tools::ShowMessageBox(const wstring& title, const wstring& text)
{
	if (messageBoxCallback != nullptr)
	{
		messageBoxCallback(title.c_str(), text.c_str());
	}
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

bool Tools::ReadFileToArray(wstring file, std::unique_ptr<char> &arr, UINT &size)
{
	std::ifstream stream = std::ifstream(file, std::ios::binary);
	if (!stream.is_open())
		return false;
	stream.seekg(0, stream.end);
	size = (UINT)stream.tellg();
	stream.seekg(0, stream.beg);
	arr = std::unique_ptr<char>(new char[size]);
	stream.read(arr.get(), size);
	stream.close();
	return true;
}