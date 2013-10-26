using System.Collections.Generic;
using System.Windows.Controls;
using System;

//jo, jo, tuoj
#pragma warning disable 169

namespace Laboras21
{
    public class SuperCanvas : Canvas
    {
        private List<Tuple<Point, Point>> edges = new List<Tuple<Point,Point>>();
        private IReadOnlyList<Vertex> nodes;

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

        private void Redraw()
        {

        }
    }
}