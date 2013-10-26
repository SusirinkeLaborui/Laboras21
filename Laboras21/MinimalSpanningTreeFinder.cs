using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laboras21
{
    public class MinimalSpanningTreeFinder
    {
        private SuperCanvas canvas;

        public MinimalSpanningTreeFinder()
        {
        }

        public MinimalSpanningTreeFinder(SuperCanvas drawableCanvas)
        {
            if (drawableCanvas == null)
            {
                throw new ArgumentException("Specified canvas was null!", "drawableCanvas");
            }

            canvas = drawableCanvas;
        }

        public async Task<List<Vertex>> FindAsync(IReadOnlyList<Point> points)
        {
            if (points.Count == 0)
            {
                return await Task.Run(() =>
                {
                    return new List<Vertex>();
                });
            }
            else
            {
                return await Task.Run(() =>
                {
                    return Find(points);
                });
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

        private List<Vertex> Find(IReadOnlyList<Point> points)
        {
            var treePoints = new List<PointInArray>(points.Count);      // Taškai, kurie jau įtraukti į medį
            var sparePoints = new List<PointInArray>(points.Count);     // Taškai, kurie dar neįtraukti į medį
            var graph = new List<Vertex>(points.Count);

            graph.Add(new Vertex(points[0]));
            treePoints.Add(new PointInArray(0, points[0]));
            
            for (int i = 1; i < points.Count; i++)
            {
                sparePoints.Add(new PointInArray(i, points[i]));
                graph.Add(new Vertex(points[i]));
            }

            if (canvas != null)
            {
                canvas.SetCollection(graph);
            }

            while (treePoints.Count < graph.Count)
            {
                int minimalDistance = GetDistanceSqr(treePoints[0].Coordinates, sparePoints[0].Coordinates);
                int minimalSparePoint = 0, 
                    minimalTreePoint = 0;

                for (int i = 0; i < treePoints.Count; i++)
                {
                    for (int j = 1; j < sparePoints.Count; j++)
                    {
                        if (GetDistanceSqr(treePoints[i].Coordinates, sparePoints[j].Coordinates) < minimalDistance)
                        {
                            minimalTreePoint = i;
                            minimalSparePoint = j;
                        }
                    }
                }

                graph[treePoints[minimalTreePoint].Index].Neighbours.Add(graph[sparePoints[minimalSparePoint].Index]);
                graph[sparePoints[minimalSparePoint].Index].Neighbours.Add(graph[treePoints[minimalTreePoint].Index]);

                DrawEdge(graph[treePoints[minimalTreePoint].Index], graph[sparePoints[minimalSparePoint].Index]);

                treePoints.Add(sparePoints[minimalSparePoint]);
                sparePoints[minimalSparePoint] = sparePoints[sparePoints.Count - 1];
                sparePoints.RemoveAt(sparePoints.Count - 1);
            }

            return graph;
        }

        private int GetDistanceSqr(Point point1, Point point2)
        {
            return (point1.x - point2.x) * (point1.x - point2.x) + (point1.y - point2.y) * (point1.y - point2.y);
        }

        private void DrawEdge(Vertex vertex1, Vertex vertex2)
        {
            if (canvas != null)
            {
                canvas.DrawEdge(vertex1, vertex2);
            }
        }
    }
}
