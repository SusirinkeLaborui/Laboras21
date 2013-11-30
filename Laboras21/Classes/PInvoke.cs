using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21.Classes
{
    internal class PInvoke
    {
        #region WINAPI


        internal const int WS_CHILD = 0x40000000,
                           WS_VISIBLE = 0x10000000,
                           LBS_NOTIFY = 0x00000001,
                           HOST_ID = 0x00000002,
                           LISTBOX_ID = 0x00000001,
                           WS_VSCROLL = 0x00200000,
                           WS_BORDER = 0x00800000;


        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
                                                      string lpszClassName,
                                                      string lpszWindowName,
                                                      int style,
                                                      int x, int y,
                                                      int width, int height,
                                                      IntPtr hwndParent,
                                                      IntPtr hMenu,
                                                      IntPtr hInst,
                                                      [MarshalAs(UnmanagedType.AsAny)] object pvParam);


        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);


        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        #endregion

        #region D3DBackend


        internal delegate void MessageBoxCallback([MarshalAs(UnmanagedType.LPWStr)] string title, [MarshalAs(UnmanagedType.LPWStr)] string text);


        [DllImport("D3DBackend.dll")]
        internal static extern void SetMessageBoxCallback(MessageBoxCallback callback);


        [DllImport("D3DBackend.dll")]
        internal static extern IntPtr CreateD3DContext(int width, int height, IntPtr hwnd);

        
        [DllImport("D3DBackend.dll")]
        internal static extern void DestroyD3DContext(ref IntPtr d3DBackendHandle);


        [DllImport("D3DBackend.dll")]
        internal static extern void ResizeWindow(IntPtr systemInstance, int newWidth, int newHeight);

        
        [DllImport("D3DBackend.dll")]
        internal static extern void DrawNodes(IntPtr systemInstance, [MarshalAs(UnmanagedType.LPArray)] Point[] nodeList, int nodeCount);

        
        [DllImport("D3DBackend.dll")]
        internal static extern void DrawSingleEdge(IntPtr systemInstance, Point nodeA, Point nodeB);
        

        [DllImport("D3DBackend.dll")]
        internal static extern void ClearNodes(IntPtr systemInstance);


        [DllImport("D3DBackend.dll")]
        internal static extern void ClearEdges(IntPtr systemInstance);

        #endregion
    }
}
