using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace Laboras21.Controls
{
    public class SuperCanvas : Canvas
    {
        private List<Tuple<Point, Point>> edges = new List<Tuple<Point,Point>>();
        private IReadOnlyList<Vertex> nodes;
        private SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private List<DispatcherOperation> drawTasks = new List<DispatcherOperation>();
        private TransformGroup transform;
        private double xFactor = 0;//bwahahahahahaha
        private double yFactor = 0;
        private int xOffset = 0;
        private int yOffset = 0;
        private const int nodeRadius = 50;
        private const int lineWidth = 30;

        private struct DoublePoint
        {
            public double x;
            public double y;
        }

        public SuperCanvas()
        {
            xOffset = (MagicalNumbers.MaxX - MagicalNumbers.MinX) / 2;
            yOffset = (MagicalNumbers.MaxY - MagicalNumbers.MinY) / 2;

            xFactor = 1000.0 / MagicalNumbers.DataWidth;
            yFactor = 1000.0 / MagicalNumbers.DataHeight;

            var translate = new TranslateTransform(xOffset, yOffset);
            var scale = new ScaleTransform(xFactor, yFactor);
            transform = new TransformGroup();
            transform.Children.Add(translate);
            transform.Children.Add(scale);

            //testing

            /*var temp = new List<Vertex>();
            temp.Add(new Vertex(new Point(0, 0)));
            temp.Add(new Vertex(new Point(7000, 5000)));
            SetCollection(temp);
            AddEdge(temp[0], temp[1]);*/
        }

        public async void SetCollection(IReadOnlyList<Vertex> vertices)
        {
            nodes = vertices;
            await Redraw();
        }

        public void AddEdge(Vertex vertex1, Vertex vertex2)
        {
            if (!InBounds(vertex1.Coordinates) || !InBounds(vertex2.Coordinates))
                return;
            Point p1 = vertex1.Coordinates;
            Point p2 = vertex2.Coordinates;
            edges.Add(new Tuple<Point, Point>(p1, p2));
            AddEdge(p1, p2);
        }

        private bool InBounds(Point p)
        {
            return p.x >= MagicalNumbers.MinX && p.x <= MagicalNumbers.MaxX && p.y >= MagicalNumbers.MinY && p.y <= MagicalNumbers.MaxY;
        }

        private void AddEdge(Point p1, Point p2)
        {
            Line l = new Line();
            l.X1 = p1.x;
            l.X2 = p2.x;
            l.Y1 = p1.y;
            l.Y2 = p2.y;
            l.Stroke = brush;
            l.StrokeThickness = lineWidth;
            l.RenderTransform = transform;

            drawTasks.Add(Dispatcher.InvokeAsync(() =>
            {
                Children.Add(l);
            }, DispatcherPriority.Background));
        }

        private void AddNode(Point p)
        {
            if (!InBounds(p))
                return;
            var node = new Ellipse();
            node.SetValue(Canvas.LeftProperty, (double)p.x);
            node.SetValue(Canvas.TopProperty, (double)p.y);
            node.Width = node.Height = nodeRadius * 2;
            node.Fill = brush;
            node.RenderTransform = transform;

            drawTasks.Add(Dispatcher.InvokeAsync(() =>
            {
                Children.Add(node);
            }, DispatcherPriority.Background));
        }

        private async Task Redraw()
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
            try
            {
                foreach (var task in drawTasks)
                    await task;
            }
            catch (Exception e)
            {
            }
            drawTasks.Clear();
        }
    }
}