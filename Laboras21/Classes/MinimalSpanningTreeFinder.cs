using Laboras21.Controls;
using Laboras21.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Laboras21
{
    public class MinimalSpanningTreeFinder
    {
        private Action<Vertex, Vertex> drawEdge;
        private Action<double> reportProgress;

        private Task currentFindTask;
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Constructs MinimalSpanningTreeFinder object without callbacks.
        /// </summary>
        public MinimalSpanningTreeFinder()
        {
        }

        /// <summary>
        /// Constructs MinimalSpanningTreeFinder object.
        /// </summary>
        /// <param name="drawEdgeCallback">Callback which is called during minimal spanning tree search when edge is confirmed to be part of minimal spanning tree. Takes two parameters - two points that the edge joins.</param>
        /// <param name="reportProgressCallback">Callback which is periodically called during minimal spanning tree search to report current progress. Takes one parameter - percentage of completion.</param>
        public MinimalSpanningTreeFinder(Action<Vertex, Vertex> drawEdgeCallback, Action<double> reportProgressCallback)
        {
            if (drawEdgeCallback == null)
            {
                throw new ArgumentException("Draw edge callback was null!", "drawEdgeCallback");
            }

            if (reportProgressCallback == null)
            {
                throw new ArgumentException("Report progress callback was null!", "reportProgressCallback");
            }

            drawEdge = drawEdgeCallback;
            reportProgress = reportProgressCallback;
        }

        /// <summary>
        /// Cancels current minimal spanning tree search. If no search is happening, has no effect.
        /// </summary>
        public void CancelSearch()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Asynchronously finds minimal spanning tree in the graph using Prim's algorithm.
        /// </summary>
        /// <param name="graph">>Graph, in which minimal spanning tree has to be found.</param>
        /// <returns>Asynchronous search task.</returns>
        public async Task FindAsync(IList<Vertex> graph)
        {
            if (currentFindTask != null)
            {
                throw new InvalidOperationException("Only one find operation may be performed at any single time on a single finder object!");
            }
            if (graph == null)
            {
                throw new ArgumentException("Graph cannot be null!", "graph");
            }

            cancellationTokenSource = new CancellationTokenSource();

            if (graph.Count == 0)
            {
                return;
            }
            else
            {
                currentFindTask = Task.Run(() =>
                    {
                        Find(graph, cancellationTokenSource.Token);
                    }, cancellationTokenSource.Token);
            }

            try
            {
                await currentFindTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (InvalidOperationException ex)
            {
                if (!(ex.InnerException is OperationCanceledException))
                {
                    throw;
                }
            }
            finally
            {
                currentFindTask = null;
                cancellationTokenSource = null;
            }
        }

        /* Duomenų struktūra
         * Programos duomenys (visi taškai) yra saugomi kaimynių sąrašų duomenų struktūroje. Viena viršūnė saugoma taip:
         *     Viršūnės koordinatės.
         *     Viršūnių kaimynių masyvas.
         * Visos viršūnės yra saugomos masyve.
         * 
         * Algoritmas
         * Programinė įranga, trumpiausiam jungiamajam medžiui surasti naudoja Primo algoritmą:
         * 1.   Duomenų struktūrų paruošimas: du masyvai: viename yra visi duoti taškai ir tušti jų kaimynių masyvai (viršūnių masyvas),
         *      ir tuščias masyvas, apsakantis, kurios viršūnės yra medyje (toliau medžio viršūnių aibė).
         * 
         * 2.   Pirmą viršūnių masyvo narį įtraukiame į medžio viršūnių aibę.
         * 
         * 3.   Kol medžio viršūnių aibės dydis yra mažesnis, nei viršūnių masyvo dydis, kartoti:
         *          - Pasirenkame tokią viršūnių porą (i; j), kad atstumas tarp jų būtų minimalus ir 
         *          viršūnė i būtų medžio viršūnių aibėje, o viršūnė j –  nebūtų. 
         *          - Įtraukiame viršūnę j į medžio viršūnių aibę ir viršūnių masyve pažymime, jog viršūnė j yra viršūnės i kaimynė
         *          ir jog viršūnė i yra viršūnės j kaimynė. 
         * 
         * 4.   Rezultatas: viršūnių masyvas, kuris yra trumpiausias jungiamasis medis.
         * 
         */

        /// <summary>
        /// struct Edge describes one graph edge: it holds its length and both points it connects.
        /// </summary>
        private struct Edge
        {
            public int Distance { get; private set; }
            public Point Coordinates1 { get; private set; }
            public Point Coordinates2 { get; private set; }

            public Edge(int distance, Point coordinates1, Point coordinates2) : this()
            {
                Distance = distance;
                Coordinates1 = coordinates1;
                Coordinates2 = coordinates2;
            }
        }

        /// <summary>
        /// Finds minimal spanning tree using Prim's algorithm.
        /// </summary>
        /// <param name="graph">Graph, in which minimal spanning tree has to be found.</param>
        /// <param name="cancellationToken">Task cancellation token. If task is cancelled using that token, this function throws OperationCancelledException.</param>
        private void Find(IList<Vertex> graph, CancellationToken cancellationToken)
        {
            var treePoints = new Dictionary<Point, int>();      // Taškai, kurie jau įtraukti į medį
            var sparePoints = new Dictionary<Point, int>();     // Taškai, kurie dar neįtraukti į medį

            treePoints.Add(graph[0].Coordinates, 0);
            for (int i = 1; i < graph.Count; i++)
            {
                sparePoints.Add(graph[i].Coordinates, i);
            }

            var distances = CalculateDistances(graph, cancellationToken);
            Array.Sort(distances, (item1, item2) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return item1.Distance.CompareTo(item2.Distance);
                });

            for (int i = 0; i < distances.Length && treePoints.Count < graph.Count; i++)
            {
                bool containsFirst = treePoints.ContainsKey(distances[i].Coordinates1),
                     containsSecond = treePoints.ContainsKey(distances[i].Coordinates2);

                if (containsFirst != containsSecond)
                {
                    var treeCoordinate = containsFirst ? distances[i].Coordinates1 : distances[i].Coordinates2;
                    var spareCoordinate = containsFirst ? distances[i].Coordinates2 : distances[i].Coordinates1;

                    int treeIndex = treePoints[treeCoordinate];
                    int spareIndex = sparePoints[spareCoordinate];

                    graph[treeIndex].Neighbours.Add(graph[spareIndex]);
                    graph[spareIndex].Neighbours.Add(graph[treeIndex]);

                    treePoints.Add(spareCoordinate, spareIndex);
                    sparePoints.Remove(spareCoordinate);

                    DrawEdge(graph[treeIndex], graph[spareIndex]);
                    ReportProgress(treePoints.Count, graph.Count);

                    i = -1;

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        /// <summary>
        /// Calculates squared distances between all vertices in the graph.
        /// </summary>
        /// <param name="graph">The graph of which distances between vertices has to be calculated.</param>
        /// <param name="cancellationToken">Task cancellation token. If task is cancelled using that token, this function throws OperationCancelledException.</param>
        /// <returns>Array of edges, that contain its length squared and both points it connects.</returns>
        private Edge[] CalculateDistances(IList<Vertex> graph, CancellationToken cancellationToken)
        {
            var distances = new Edge[graph.Count * (graph.Count - 1) / 2];
            var parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cancellationToken;

            Parallel.For(1, graph.Count, parallelOptions, (u) =>
                {
                    int i = (int)u;
                    int startIndex = (i - 1) * i / 2;
                    for (int j = 0; j < i; j++)
                    {
                        int distance = GetDistanceSqr(graph[i].Coordinates, graph[j].Coordinates);
                        distances[startIndex + j] = new Edge(distance, graph[i].Coordinates, graph[j].Coordinates);
                    }
                });

            return distances;
        }

        /// <summary>
        /// Returns distance between two points squared.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private int GetDistanceSqr(Point point1, Point point2)
        {
            return (point1.x - point2.x) * (point1.x - point2.x) + (point1.y - point2.y) * (point1.y - point2.y);
        }

        /// <summary>
        /// Draws an edge using a callback that was passed to this object in constructor.
        /// </summary>
        /// <param name="vertex1">One of the vertices that the edge connects.</param>
        /// <param name="vertex2">Another of the vertices that the edge connects.</param>
        private void DrawEdge(Vertex vertex1, Vertex vertex2)
        {
            if (drawEdge != null)
            {
                drawEdge(vertex1, vertex2);
            }
        }

        /// <summary>
        /// Reports minimal spanning tree search progress to callback that was passed to this object in constructor.
        /// </summary>
        /// <param name="nodesInTree">Number of vertices that are currently connected to the tree.</param>
        /// <param name="totalNodes">Number of vertices in the graph.</param>
        private void ReportProgress(long nodesInTree, long totalNodes)
        {
            if (reportProgress == null)
            {
                return;
            }

            double progress = 100 * (double)nodesInTree / (double)totalNodes;
            reportProgress(progress);
        }

        public double MeasureTreeLength(IList<Vertex> Tree)
        {
            List<Edge> TreeEdges = new List<Edge>();
            foreach (var vertex in Tree)
            {
                foreach (var neighbour in vertex.Neighbours)
                {
                    Edge e = new Edge(0, vertex.Coordinates, neighbour.Coordinates);
                    // If this edge or reversed edge is not in NotMeasured List add to it
                    if (!TreeEdges.Any(x => x.Coordinates1.x == e.Coordinates1.x && x.Coordinates1.y == e.Coordinates1.y
                        && x.Coordinates2.x == e.Coordinates2.x && x.Coordinates2.y == e.Coordinates2.y ||
                         x.Coordinates2.x == e.Coordinates1.x && x.Coordinates2.y == e.Coordinates1.y
                        && x.Coordinates1.x == e.Coordinates2.x && x.Coordinates1.y == e.Coordinates2.y))
                    {
                        TreeEdges.Add(e);
                    }
                }
            }

            double length = 0;
            foreach (var edge in TreeEdges)
            {
                length += Math.Sqrt(GetDistanceSqr(edge.Coordinates1, edge.Coordinates2));
            }
            return length;
        }
    }
}
