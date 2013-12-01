using Laboras21.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Laboras21.Controls
{
    public class UberCanvas : HwndHost
    {
        private IntPtr hwndHost;
        private IntPtr d3DWindowHandle;
        private PInvoke.RawInputCallback inputCallback;

        public UberCanvas()
        {
            Loaded += (sender, e) => { InitD3D(); };

            inputCallback = (wParam, lParam) =>
                {
                    if (d3DWindowHandle != null)
                    {
                        PInvoke.HandleRawInput(d3DWindowHandle, wParam, lParam);
                    }
                };
        }
        
        private async void InitD3D()
        {
            await Task.Delay(500);  // Wait for metro animation to finish, otherwise window spawns at wrong position.

            Visibility = Visibility.Visible;

            // Wait for next frame, as actual width and height are zeroes because it's invisible
            await Dispatcher.InvokeAsync(() =>
            {
                Resize(null, null);
            }, DispatcherPriority.Render);

            var backgroundColor = (FindBackgroundColor(this) as SolidColorBrush).Color;

            d3DWindowHandle = PInvoke.CreateD3DContext((int)ActualWidth, (int)ActualHeight,
                backgroundColor.R, backgroundColor.G, backgroundColor.B, hwndHost);

            PInvoke.RunD3DContextAsync(d3DWindowHandle);
        }

        protected Brush FindBackgroundColor(FrameworkElement element)
        {
            var control = element as Control;
            if (control == null)
            {
                return FindBackgroundColor(element.Parent as FrameworkElement);
            }
            else
            {
                return control.Background;
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var backgroundColor = (FindBackgroundColor(this) as SolidColorBrush).Color;
            hwndHost = PInvoke.CreateColoredWindow(hwndParent.Handle, backgroundColor.R, backgroundColor.G, backgroundColor.B, inputCallback);

            SizeChanged += Resize;
            Visibility = Visibility.Collapsed;

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
                    PInvoke.DrawSingleEdge(d3DWindowHandle, p1, p2);
                });
        }
    }
}