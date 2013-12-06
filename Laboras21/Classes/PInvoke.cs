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

        
        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);


        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);


        #endregion

        #region D3DBackend


        internal delegate void MessageBoxCallback([MarshalAs(UnmanagedType.LPWStr)] string title, [MarshalAs(UnmanagedType.LPWStr)] string text);
        internal delegate void RawInputCallback(int wParam, int lParam);


        [DllImport("D3DBackend.dll")]
        internal static extern void SetMessageBoxCallback(MessageBoxCallback callback);


        [DllImport("D3DBackend.dll")]
        internal static extern IntPtr CreateD3DContext(int width, int height, int backgroundR, int backgroundG, int backgroundB, IntPtr parentHwnd);


        [DllImport("D3DBackend.dll")]
        internal static extern void RunD3DContextAsync(IntPtr d3dBackendHandle);


        [DllImport("D3DBackend.dll")]
        internal static extern void DestroyD3DContext(ref IntPtr d3DBackendHandle);


        [DllImport("D3DBackend.dll")]
        internal static extern void ResizeWindow(IntPtr systemInstance, int newWidth, int newHeight);

        
        [DllImport("D3DBackend.dll")]
        internal static extern void DrawNodes(IntPtr systemInstance, [MarshalAs(UnmanagedType.LPArray)] Point[] nodeList, int nodeCount);


        [DllImport("D3DBackend.dll")]
        internal static extern void DrawSingleEdge(IntPtr systemInstance, Point nodeA, Point nodeB);


        [DllImport("D3DBackend.dll")]
        internal static extern void DrawManyEdges(IntPtr systemInstance, [MarshalAs(UnmanagedType.LPArray)] Point[] edgeList, int nodeCount);
        

        [DllImport("D3DBackend.dll")]
        internal static extern void ClearNodes(IntPtr systemInstance);


        [DllImport("D3DBackend.dll")]
        internal static extern void ClearEdges(IntPtr systemInstance);


        [DllImport("D3DBackend.dll")]
        internal static extern IntPtr CreateColoredWindow(IntPtr parent, int r, int g, int b, RawInputCallback rawInputCallback);


        [DllImport("D3DBackend.dll")]
        internal static extern void HandleRawInput(IntPtr systemInstance, int wParam, int lParam);

        [DllImport("D3DBackend.dll")]
        internal static extern float GetNodeSize();

        #endregion
    }
}
