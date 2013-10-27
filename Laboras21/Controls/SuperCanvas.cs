using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Laboras21
{
    public class SuperCanvas : Canvas
    {
        private List<Tuple<Point, Point>> edges = new List<Tuple<Point,Point>>();
        private IReadOnlyList<Vertex> nodes;
        private SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public void SetCollection(IReadOnlyList<Vertex> vertices)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                nodes = vertices;
                Redraw();
            }));
        }

        public void DrawEdge(Vertex vertex1, Vertex vertex2)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Point p1 = vertex1.Coordinates;
                Point p2 = vertex2.Coordinates;
                edges.Add(new Tuple<Point, Point>(p1, p2));
                DrawEdge(p1, p2);
            }));
        }

        private void DrawEdge(Point point1, Point point2)
        {

        }

        private void DrawNode(Point point)
        {
            point.x += MagicalNumbers.CoordWidth / 2;
            point.y += MagicalNumbers.CoordWidth / 2;
            Ellipse e = new Ellipse();
            e.Fill = brush;
        }

        private void Redraw()
        {
            foreach(var n in nodes)
                DrawNode(n.Coordinates);

            foreach (var e in edges)
                DrawEdge(e.Item1, e.Item2);
        }
    }
}