#include <algorithm>
#include <ctime>
#include <fstream>
#include <memory>
#include <mutex>
#include <set>
#include <sstream>
#include <string>
#include <vector>
#include <set>
#include <thread>
#include <unordered_map>
#include <mutex>
#include <condition_variable>

#include <dxgi.h>

#include <d3dcommon.h>
#include <d3d11.h>
#include <DirectXMath.h>

#include <wrl\client.h>

#define WIDE2(x) L##x
#define WIDE1(x) WIDE2(x)
#define __WFILE__ WIDE1(__FILE__)

using namespace std;
using namespace Microsoft::WRL;