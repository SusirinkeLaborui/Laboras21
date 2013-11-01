using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading;

namespace Laboras21.Controls
{
    public class SuperCanvas : Canvas
    {
        private List<Tuple<Point, Point>> edges = new List<Tuple<Point,Point>>();
        private IReadOnlyList<Vertex> nodes;
        private SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private List<DispatcherOperation> drawTasks = new List<DispatcherOperation>();
        private double xFactor = 0;//bwahahahahahaha
        private double yFactor = 0;
        private int xOffset = 0;
        private int yOffset = 0;
        private const int nodeRadius = 4;
        private const int lineWidth = 2;

        private struct DoublePoint
        {
            public double x;
            public double y;
        }

        public SuperCanvas()
        {
            SizeChanged += SuperCanvas_SizeChanged;
            xOffset = (MagicalNumbers.MaxX - MagicalNumbers.MinX) / 2;
            yOffset = (MagicalNumbers.MaxY - MagicalNumbers.MinY) / 2;

            //testing

            /*var temp = new List<Vertex>();
            temp.Add(new Vertex(new Point(0, 0)));
            temp.Add(new Vertex(new Point(4000, 2000)));
            SetCollection(temp);
            DrawEdge(temp[0], temp[1]);*/
        }

        void SuperCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            xFactor = e.NewSize.Width / MagicalNumbers.DataWidth;
            yFactor = e.NewSize.Height / MagicalNumbers.DataHeight;
            Refresh();
        }

        public void SetCollection(IReadOnlyList<Vertex> vertices)
        {
            nodes = vertices;
            Redraw();
        }

        public void AddEdge(Vertex vertex1, Vertex vertex2)
        {
            Point p1 = vertex1.Coordinates;
            Point p2 = vertex2.Coordinates;
            edges.Add(new Tuple<Point, Point>(p1, p2));
            AddEdge(p1, p2);
        }

        private void AddEdge(Point p1, Point p2)
        {
            drawTasks.Add(Dispatcher.InvokeAsync(() =>
            {
                Line l = new Line();
                l.X1 = p1.x;
                l.X2 = p2.x;
                l.Y1 = p1.y;
                l.Y2 = p2.y;
                l.Stroke = brush;
                l.StrokeThickness = lineWidth;
                Children.Add(l);
            }));
        }

        private void AddNode(Point p)
        {
            drawTasks.Add(Dispatcher.InvokeAsync(() =>
            {
                var node = new Ellipse();
                node.SetValue(Canvas.LeftProperty, (double)(p.x - nodeRadius));
                node.SetValue(Canvas.TopProperty, (double)(p.y - nodeRadius));
                node.Width = node.Height = nodeRadius * 2;
                node.Fill = brush;

                Children.Add(node);
            }));
        }

        private DoublePoint Translate(Point point)
        {
            DoublePoint p;
            p.x = point.x + xOffset;
            p.x *= xFactor;
            p.y = point.y + yOffset;
            p.y *= yFactor;
            return p;
        }

        private async void Refresh()
        {
            var translate = new TranslateTransform(xOffset, yOffset);
            var scale = new ScaleTransform(xFactor, yFactor);
            var transform = new TransformGroup();
            transform.Children.Add(translate);
            transform.Children.Add(scale);

            int n = await Dispatcher.InvokeAsync<int>(() =>
            {
                return Children.Count;
            });

            for (int i = 0; i < n; i++)
            {
                drawTasks.Add(Dispatcher.InvokeAsync(() =>
                {
                    Children[i].RenderTransform = transform;
                }, DispatcherPriority.Background));
            }
        }

        private async void Redraw()
        {
            foreach (var t in drawTasks)
                t.Abort();
            drawTasks.Clear();
            await Dispatcher.InvokeAsync(() =>
            {
                Children.Clear();
            });

            if (nodes != null)
            {
                foreach (var n in nodes)
                    AddNode(n.Coordinates);

                foreach (var e in edges)
                    AddEdge(e.Item1, e.Item2);
            }
            foreach (var task in drawTasks)
                await task;
            drawTasks.Clear();
            Refresh();
        }
    }
}