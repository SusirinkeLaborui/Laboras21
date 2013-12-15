using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Laboras21;
using System.Linq;

namespace Test_framework
{
    [TestClass]
    public class MinimalSpanningTreeTest
    {
        public static List<Vertex> graph;
        
        [ClassInitialize]
        public static void Initialize(TestContext t)
        {
            graph = new List<Vertex>();
            graph.Add(new Vertex(new Point(0, 0)));
            graph.Add(new Vertex(new Point(0, 100)));
            graph.Add(new Vertex(new Point(0, 200)));
            graph.Add(new Vertex(new Point(0, 300)));
            graph.Add(new Vertex(new Point(100, 100)));
            graph.Add(new Vertex(new Point(100, 200)));
            graph.Add(new Vertex(new Point(200, 200)));
        }

        [TestMethod]
        public void MeasureTreeLengthTest()
        {
            var finder = new MinimalSpanningTreeFinder();
            finder.FindAsync(graph).Wait();

            Assert.AreEqual(finder.MeasureTreeLength(graph), 600);
        }

        private float minLength = float.MaxValue;

        private void MakePairs(ref List<Tuple<Vertex, Vertex>> chosenPairs, List<Tuple<Vertex, Vertex>> pairs, int currentPos, int from, int n)
        {
	        if (currentPos == n)
	        {
		        if (IsSpanningTree(ref chosenPairs, n))
		        {
                    foreach (var numberPair in chosenPairs)
                    {
                        var length = chosenPairs.Sum(x => Math.Sqrt(MinimalSpanningTreeFinder.GetDistanceSqr(x.Item1.Coordinates, x.Item2.Coordinates)));
                        if (length < minLength)
                            minLength = length;
			        }
		        }
	        }
	        else
	        {
		        for (var i = from; i < pairs.Count; i++)
		        {
			        chosenPairs[currentPos] = pairs[i];
			        MakePairs(ref chosenPairs, pairs, currentPos + 1, i + 1, n);
		        }
	        }
        }

        private bool IsSpanningTree(ref List<Tuple<Vertex, Vertex>> edges, int n)
        {
	        var vertices = new List<Vertex>(n + 1);
	        int numberOfVerticesVisited = 0;
            Queue<Vertex> toVisit;

	        foreach (var edge in edges)
	        {
		        vertices[edge.Item1].Neighbours.Add(edge.Item2);
		        vertices[edge.Item2].Neighbours.Add(edge.Item1);
	        }

	        if (n > 0)
	        {
                toVisit.Enqueue(vertices[0]);
		        vertices[0].Visited = true;
		        numberOfVerticesVisited++;
		
		        while (toVisit.Count > 0)
		        {
			        var vertexx = toVisit.Dequeue();

                    foreach (var vertice in vertexx.Neighbours)
			        {
				        if (!vertice.Visited)
				        {
                            toVisit.Enqueue(vertice);
					        vertice.Visited = true;
					        numberOfVerticesVisited++;
				        }
			        }
		        }
	        }
	        return numberOfVerticesVisited == n + 1;
        }
    }
}
