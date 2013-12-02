using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Laboras21;

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
    }
}
