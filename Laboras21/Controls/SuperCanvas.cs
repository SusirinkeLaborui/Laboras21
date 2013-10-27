﻿using System.Collections.Generic;
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
        private double xFactor = 0;//bwahahahahahaha
        private double yFactor = 0;
        private int xOffset = 0;
        private int yOffset = 0;
        private const int nodeRadius = 4;

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
            /*var temp = new List<Vertex>();
            temp.Add(new Vertex(new Point(0, 0)));
            SetCollection(temp);*/
        }

        void SuperCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            xFactor = e.NewSize.Width / MagicalNumbers.DataWidth;
            yFactor = e.NewSize.Height / MagicalNumbers.DataHeight;
            Redraw();
        }

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
            var node = new Ellipse();
            var p = Translate(point);
            node.SetValue(Canvas.LeftProperty, p.x - nodeRadius);
            node.SetValue(Canvas.TopProperty, p.y - nodeRadius);
            node.Width = node.Height = nodeRadius * 2;
            node.Fill = brush;

            Children.Add(node);
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

        private void Redraw()
        {
            Children.Clear();
            if (nodes != null)
            {
                foreach (var n in nodes)
                    DrawNode(n.Coordinates);

                foreach (var e in edges)
                    DrawEdge(e.Item1, e.Item2);
            }
        }
    }
}