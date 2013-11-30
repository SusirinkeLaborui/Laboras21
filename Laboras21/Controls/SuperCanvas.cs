using Laboras21.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Laboras21.Controls
{
    public class SuperCanvas : HwndHost
    {
        private IntPtr hwndHost;
        private IntPtr d3DWindowHandle;

        public SuperCanvas()
        {
            //testing

            /*var temp = new List<Vertex>();
            temp.Add(new Vertex(new Point(0, 0)));
            temp.Add(new Vertex(new Point(7000, 5000)));
            SetCollection(temp);

            AddEdge(temp[0], temp[1]);*/

            Visibility = System.Windows.Visibility.Visible;
            Loaded += (sender, e) => { InitD3D(); };
        }
        
        private async void InitD3D()
        {
            int width = (int)ActualWidth,
                height = (int)ActualHeight;

            Visibility = System.Windows.Visibility.Collapsed;
            await Task.Delay(500);  // Wait for metro animation to finish, otherwise window spawns at wrong position.

            Visibility = System.Windows.Visibility.Visible;
            Resize(null, null);
            d3DWindowHandle = PInvoke.CreateD3DContext(width, height, hwndHost);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            hwndHost = PInvoke.CreateWindowEx(0, "static", "", PInvoke.WS_CHILD, 0, 0, (int)ActualWidth, (int)ActualHeight,
                hwndParent.Handle, (IntPtr)PInvoke.HOST_ID, IntPtr.Zero, 0);

            SizeChanged += Resize;

            return new HandleRef(this, hwndHost);
        }
        
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            SizeChanged -= Resize;

            if (d3DWindowHandle != IntPtr.Zero)
            {
                PInvoke.DestroyD3DContext(ref d3DWindowHandle);
            }

            PInvoke.DestroyWindow(hwnd.Handle);
        }

        private void Resize(object sender, System.Windows.SizeChangedEventArgs e)
        {
            PInvoke.MoveWindow(hwndHost, 0, 0, (int)ActualWidth, (int)ActualHeight, true);

            if (d3DWindowHandle != IntPtr.Zero)
            {
                PInvoke.ResizeWindow(d3DWindowHandle, (int)e.NewSize.Width, (int)e.NewSize.Height);
            }
        }

        /// <summary>
        /// Clears all edges from the canvas, thread safe
        /// </summary>
        public async Task ClearEdgesAsync()
        {
            await Task.Run(() =>
                {
                    PInvoke.ClearEdges(d3DWindowHandle);
                });
        }

        /// <summary>
        /// Replaces the current nodes with vertices provided
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public async Task SetCollectionAsync(IReadOnlyList<Vertex> vertices)
        {
            await ClearEdgesAsync();

            await Task.Run(() =>
                {
                    PInvoke.ClearNodes(d3DWindowHandle);
                    PInvoke.DrawNodes(d3DWindowHandle, vertices.Select(x => x.Coordinates).ToArray<Point>(), vertices.Count);
                });            
        }
        
        /// <summary>
        /// Adds an edge, can be called from wherever
        /// </summary>
        /// <param name="vertex1">start</param>
        /// <param name="vertex2">end</param>
        public async void AddEdgeAsync(Vertex vertex1, Vertex vertex2)
        {
            var p1 = vertex1.Coordinates;
            var p2 = vertex2.Coordinates;

            await Task.Run(() =>
            {
                PInvoke.DrawEdge(d3DWindowHandle, p1, p2);
            });
        }
    }
}