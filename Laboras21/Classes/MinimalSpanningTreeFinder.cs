using Laboras21.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laboras21
{
    public class MinimalSpanningTreeFinder
    {
        private SuperCanvas canvas;
        private Task currentFindTask;
        CancellationTokenSource cancellationTokenSource;

        public MinimalSpanningTreeFinder()
        {
        }

        public MinimalSpanningTreeFinder(SuperCanvas drawableCanvas)
        {
            if (drawableCanvas == null)
            {
                throw new ArgumentException("Canvas was null!", "drawableCanvas");
            }

            canvas = drawableCanvas;
        }

        public void CancelSearch()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

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

        private struct PointInArray
        {
            public int Index { get; private set; }
            public Point Coordinates { get; private set; }

            public PointInArray(int index, Point coordinates) : this()
            {
                Index = index;
                Coordinates = coordinates;
            }
        }

        private void Find(IList<Vertex> graph, CancellationToken cancellationToken)
        {
            var treePoints = new List<PointInArray>(graph.Count);      // Taškai, kurie jau įtraukti į medį
            var sparePoints = new List<PointInArray>(graph.Count);     // Taškai, kurie dar neįtraukti į medį

            treePoints.Add(new PointInArray(0, graph[0].Coordinates));            
            for (int i = 1; i < graph.Count; i++)
            {
                sparePoints.Add(new PointInArray(i, graph[i].Coordinates));
            }
            
            while (treePoints.Count < graph.Count)
            {
                int minimalDistance = GetDistanceSqr(treePoints[0].Coordinates, sparePoints[0].Coordinates);
                int minimalSparePoint = 0, 
                    minimalTreePoint = 0;

                for (int i = 0; i < treePoints.Count; i++)
                {
                    for (int j = 0; j < sparePoints.Count; j++)
                    {
                        if (GetDistanceSqr(treePoints[i].Coordinates, sparePoints[j].Coordinates) < minimalDistance)
                        {
                            minimalTreePoint = i;
                            minimalSparePoint = j;
                        }
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }

                graph[treePoints[minimalTreePoint].Index].Neighbours.Add(graph[sparePoints[minimalSparePoint].Index]);
                graph[sparePoints[minimalSparePoint].Index].Neighbours.Add(graph[treePoints[minimalTreePoint].Index]);

                DrawEdge(graph[treePoints[minimalTreePoint].Index], graph[sparePoints[minimalSparePoint].Index]);

                treePoints.Add(sparePoints[minimalSparePoint]);
                sparePoints[minimalSparePoint] = sparePoints[sparePoints.Count - 1];
                sparePoints.RemoveAt(sparePoints.Count - 1);
            }
        }

        private int GetDistanceSqr(Point point1, Point point2)
        {
            return (point1.x - point2.x) * (point1.x - point2.x) + (point1.y - point2.y) * (point1.y - point2.y);
        }

        private void DrawEdge(Vertex vertex1, Vertex vertex2)
        {
            if (canvas != null)
            {
                canvas.AddEdge(vertex1, vertex2);
            }
        }
    }
}
