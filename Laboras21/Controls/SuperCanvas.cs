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
using System.Diagnostics;

namespace Laboras21.Controls
{
    public class SuperCanvas : Canvas
    {
        private List<Tuple<Point, Point>> edges = new List<Tuple<Point,Point>>();
        private IReadOnlyList<Vertex> nodes;
        private SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private HashSet<DispatcherOperation> drawTasks = new HashSet<DispatcherOperation>();
        private List<CancellationTokenSource> cancellationTokenSources = new List<CancellationTokenSource>();
        private Task nodeRedrawTask;
        private TransformGroup transform;
        private int xOffset = 0;
        private int yOffset = 0;
        private const int nodeRadius = 50;
        private const int lineWidth = 20;

        public ProgressBar ProgressBar { get; set; } 

        public SuperCanvas()
        {
            //testing

            /*var temp = new List<Vertex>();
            temp.Add(new Vertex(new Point(0, 0)));
            temp.Add(new Vertex(new Point(7000, 5000)));
            SetCollection(temp);

            AddEdge(temp[0], temp[1]);*/
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            xOffset = -MagicalNumbers.MinX + nodeRadius;
            yOffset = -MagicalNumbers.MinY + nodeRadius;

            var translate = new TranslateTransform(xOffset, yOffset);
            transform = new TransformGroup();
            transform.Children.Add(translate);

            Width = MagicalNumbers.DataWidth + nodeRadius * 2;
            Height = MagicalNumbers.DataHeight + nodeRadius * 2;
        }

        public async Task SetCollection(IReadOnlyList<Vertex> vertices)
        {
            lock (cancellationTokenSources)
            {
                foreach (var tokenSource in cancellationTokenSources)
                {
                    tokenSource.Cancel();
                }

                cancellationTokenSources.Clear();
            }

            // Wait for current drawing operation to end before setting nodes to new value
            if (nodeRedrawTask != null)
            {
                try
                {
                    await nodeRedrawTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
            nodes = vertices;

            var cancellationTokenSource = new CancellationTokenSource();
            lock (cancellationTokenSources)
            {
                cancellationTokenSources.Add(cancellationTokenSource);
            }

            nodeRedrawTask = RedrawNodes(cancellationTokenSource.Token);
            await nodeRedrawTask;
        }

        public async Task FinishDrawing()
        {
            while (drawTasks.Count > 0)
            {
                DispatcherOperation task = null;
                lock (drawTasks)
                {
                    var enumerator = drawTasks.GetEnumerator();

                    if (enumerator.MoveNext() != false)
                    {
                        task = enumerator.Current;
                    }                    
                }

                if (task != null)
                {
                    await task;
                }
            }
        }

        public void AddEdge(Vertex vertex1, Vertex vertex2)
        {
            edges.Add(new Tuple<Point, Point>(vertex1.Coordinates, vertex2.Coordinates));
            AddEdge(vertex1.Coordinates, vertex2.Coordinates);
        }

        private bool InBounds(Point p)
        {
            return p.x >= MagicalNumbers.MinX && p.x <= MagicalNumbers.MaxX && p.y >= MagicalNumbers.MinY && p.y <= MagicalNumbers.MaxY;
        }

        private void AddEdge(Point p1, Point p2)
        {
            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                Line l = new Line();
                l.X1 = p1.x;
                l.X2 = p2.x;
                l.Y1 = p1.y;
                l.Y2 = p2.y;
                l.Stroke = brush;
                l.StrokeThickness = lineWidth;
                l.RenderTransform = transform;

                Children.Add(l);
            }, DispatcherPriority.Background);

            lock (drawTasks)
            {
                drawTask.Completed += drawTask_Completed;
                drawTasks.Add(drawTask);
            }
        }

        private void AddNode(Point p)
        {
            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                var node = new Ellipse();
                node.SetValue(Canvas.LeftProperty, (double)p.x - nodeRadius);
                node.SetValue(Canvas.TopProperty, (double)p.y - nodeRadius);
                node.Width = node.Height = nodeRadius * 2;
                node.Fill = brush;
                node.RenderTransform = transform;

                Children.Add(node);
            }, DispatcherPriority.Background);

            lock (drawTasks)
            {
                drawTask.Completed += drawTask_Completed;
                drawTasks.Add(drawTask);
            }
        }

        private void BatchAddNodes(int from, int to, List<DispatcherOperation> nodeDrawTasks)
        {
            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                for (int i = from; i < to; i++)
                {
                    var node = new Ellipse();
                    node.SetValue(Canvas.LeftProperty, (double)nodes[i].Coordinates.x - nodeRadius);
                    node.SetValue(Canvas.TopProperty, (double)nodes[i].Coordinates.y - nodeRadius);
                    node.Width = node.Height = nodeRadius * 2;
                    node.Fill = brush;
                    node.RenderTransform = transform;

                    Children.Add(node);
                }
            }, DispatcherPriority.Background);

            nodeDrawTasks.Add(drawTask);
        }

        private void drawTask_Completed(object sender, EventArgs e)
        {
            lock (drawTasks)
            {
                drawTasks.Remove(sender as DispatcherOperation);
            }
        }

        private async Task RedrawNodes(CancellationToken cancellationToken)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Children.Clear();
            }).Task.ConfigureAwait(false);

            if (nodes == null)
            {
                return;
            }
            
            var nodeDrawTasks = new List<DispatcherOperation>();

            try
            {
                var batchCount = 25;
                for (int i = 0; batchCount * i < nodes.Count; i++)
                {
                    BatchAddNodes(i * batchCount, i * batchCount + batchCount, nodeDrawTasks);
                }
                if (nodes.Count % batchCount != 0)
                {
                    BatchAddNodes(nodes.Count - nodes.Count % batchCount, nodes.Count, nodeDrawTasks);
                }

                for (int i = 0; i < nodeDrawTasks.Count; i++)
                {
                    await nodeDrawTasks[i];
                    ReportProgress(i, nodeDrawTasks.Count);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                foreach (var operation in nodeDrawTasks)
                {
                    operation.Abort();
                }
                throw;
            }
        }

        private void ReportProgress(int nodesDrawn, int totalNodes)
        {
            if (ProgressBar == null)
            {
                return;
            }

            Dispatcher.InvokeAsync(() =>
                {
                    ProgressBar.Value = 100.0 * (double)nodesDrawn / (double)totalNodes;
                }, DispatcherPriority.Input);
        }
    }
}