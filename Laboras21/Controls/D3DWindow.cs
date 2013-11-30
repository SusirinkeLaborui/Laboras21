using Laboras21.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Laboras21.Controls
{
    public class D3DWindow : HwndHost
    {
        IntPtr hwndHost;
        IntPtr d3DWindowHandle;
        int hostHeight, hostWidth;

        public D3DWindow(double width, double height)
        {
            hostHeight = (int)height;
            hostWidth = (int)width;
        }
        
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            hwndHost = IntPtr.Zero;
            hwndHost = PInvoke.CreateWindowEx(0, "static", "", PInvoke.WS_CHILD | PInvoke.WS_VISIBLE, 0, 0, hostWidth, hostHeight,
                hwndParent.Handle, (IntPtr)PInvoke.HOST_ID, IntPtr.Zero, 0);

            d3DWindowHandle = PInvoke.CreateD3DContext(hostWidth, hostHeight, hwndHost);

            return new HandleRef(this, hwndHost);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            PInvoke.DestroyD3DContext(ref d3DWindowHandle);
            PInvoke.DestroyWindow(hwnd.Handle);
        }
    }
}
