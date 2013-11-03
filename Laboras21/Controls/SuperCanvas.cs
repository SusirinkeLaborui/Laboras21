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
        private const int maxChildCount = 200;
        private Canvas currentCanvas;
        private Canvas CurrentCanvas
        {
            set
            {
                currentCanvas = value;
            }
            get
            {
                if (currentCanvas == null || Children.Count == 0)
                {
                    currentCanvas = new Canvas();
                    Children.Add(currentCanvas);
                }
                if (currentCanvas.Children.Count >= maxChildCount)
                {
                    currentCanvas = new Canvas();
                    Children.Add(currentCanvas);
                }
                return currentCanvas;
            }
        }
        private Canvas edgeCanvas;

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

            currentCanvas = new Canvas();
            edgeCanvas = new Canvas();
            Children.Add(edgeCanvas);
        }

        /// <summary>
        /// Clears all edges from the canvas, thread safe
        /// </summary>
        public async Task ClearEdgesAsync()
        {
            CancelDrawing();
            lock (edges)
            {
                edges.Clear();
            }
            await Dispatcher.InvokeAsync(() =>
                {
                    edgeCanvas.Children.Clear();
                });
        }

        /// <summary>
        /// Replaces the current nodes with vertices provided
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public async Task SetCollectionAsync(IReadOnlyList<Vertex> vertices)
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

            nodeRedrawTask = RedrawNodesAsync(cancellationTokenSource.Token);
            await nodeRedrawTask;
        }

        /// <summary>
        /// Waits for the drawing to finish
        /// </summary>
        /// <returns></returns>
        public async Task FinishDrawingAsync()
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
                    try
                    {
                        await task;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        }

        public void CancelDrawing()
        {
            lock (drawTasks)
            {
                foreach (var task in drawTasks)
                {
                    task.Abort();
                }
                drawTasks.Clear();
            }
        }

        /// <summary>
        /// Adds an edge, can be called from wherever
        /// </summary>
        /// <param name="vertex1">start</param>
        /// <param name="vertex2">end</param>
        public void AddEdge(Vertex vertex1, Vertex vertex2)
        {
            var p1 = vertex1.Coordinates;
            var p2 = vertex2.Coordinates;
            lock (edges)
            {
                edges.Add(new Tuple<Point, Point>(p1, p2));
            }

            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                AddEdgeToCanvas(p1, p2);

            }, DispatcherPriority.Background);

            lock (drawTasks)
            {
                drawTask.Completed += drawTask_Completed;
                drawTasks.Add(drawTask);
            }
        }

        /// <summary>
        /// Adds an edge to the canvas, to be called from the UI thread
        /// </summary>
        /// <param name="p1">start</param>
        /// <param name="p2">end</param>
        private void AddEdgeToCanvas(Point p1, Point p2)
        {
            Line l = new Line();
            l.X1 = p1.x;
            l.X2 = p2.x;
            l.Y1 = p1.y;
            l.Y2 = p2.y;
            l.Stroke = brush;
            l.StrokeThickness = lineWidth;
            l.RenderTransform = transform;

            edgeCanvas.Children.Add(l);
        }

        /// <summary>
        /// Adds a node to the canvas, can be called from wherever
        /// </summary>
        /// <param name="p"></param>
        private void AddNode(Point p)
        {
            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                AddNodeToCanvas(p);
            }, DispatcherPriority.Background);

            lock (drawTasks)
            {
                drawTask.Completed += drawTask_Completed;
                drawTasks.Add(drawTask);
            }
        }

        /// <summary>
        /// Adds nodes to the canvas, from the nodes list
        /// </summary>
        /// <param name="from">index of the first to add</param>
        /// <param name="to">index after the last to add</param>
        /// <param name="nodeDrawTasks">a list to be filled with draw tasks</param>
        private void BatchAddNodes(int from, int to, List<DispatcherOperation> nodeDrawTasks)
        {
            var drawTask = Dispatcher.InvokeAsync(() =>
            {
                for (int i = from; i < to; i++)
                {
                    AddNodeToCanvas(nodes[i].Coordinates);
                }
            }, DispatcherPriority.Background);

            nodeDrawTasks.Add(drawTask);
        }

        /// <summary>
        /// Does the actual node-related UI stuff, explodes unless called from the UI thread
        /// </summary>
        /// <param name="p"></param>
        private void AddNodeToCanvas(Point p)
        {
            var node = new Ellipse();
            node.SetValue(Canvas.LeftProperty, (double)p.x - nodeRadius);
            node.SetValue(Canvas.TopProperty, (double)p.y - nodeRadius);
            node.Width = node.Height = nodeRadius * 2;
            node.Fill = brush;
            node.RenderTransform = transform;

            CurrentCanvas.Children.Add(node);
        }

        private void drawTask_Completed(object sender, EventArgs e)
        {
            lock (drawTasks)
            {
                drawTasks.Remove(sender as DispatcherOperation);
            }
        }

        /// <summary>
        /// Removes all nodes, adds new nodes from the node list
        /// </summary>
        /// <param name="cancellationToken">token that can be used to cancel this task</param>
        /// <returns></returns>
        private async Task RedrawNodesAsync(CancellationToken cancellationToken)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Children.Clear();
                edgeCanvas = new Canvas();
                Children.Add(edgeCanvas);
            }).Task.ConfigureAwait(false);

            if (nodes == null)
            {
                return;
            }

            lock (edges)
            {
                edges.Clear();
            }
            
            var nodeDrawTasks = new List<DispatcherOperation>();

            try
            {
                var batchSize = 10;
                var added = 0;
                for(; added + batchSize <= nodes.Count; added += batchSize)
                {
                    BatchAddNodes(added, added + batchSize, nodeDrawTasks);
                }
                if (added < nodes.Count)
                {
                    BatchAddNodes(added, nodes.Count, nodeDrawTasks);
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