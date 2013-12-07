using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Laboras21;

namespace Test_framework
{
    [TestClass]
    public class DataProviderTest
    {
        public static string ReadingFile;
        public static string WritingFile;
        public static List<Vertex> Data;
        public static List<Vertex> Tree;

        [ClassInitialize]
        public static void InitializeTest(TestContext t)
        {
            ReadingFile = System.IO.Path.GetTempFileName();
            WritingFile = System.IO.Path.GetTempFileName();
            
            Data = new List<Vertex>();
            for (int i = 0; i < 100; i++)
            {
                Data.Add(new Vertex(new Point(i, -i)));
            }

            Tree = new List<Vertex>();
            for (int i = 0; i < 100; i++)
            {
                Tree.Add(new Vertex(new Point(i, -i)));
                for(int j = 0; j < 5; j++)
                {
                    Tree[i].Neighbours.Add(Data[j]);
                }
            }

            using (StreamWriter file = new StreamWriter(ReadingFile))
            {
                foreach (Vertex v in Data)
                {
                    file.WriteLine(String.Format("{0} {1}", v.Coordinates.x, v.Coordinates.y));
                }
            }
        }

        [TestMethod]
        public void TestSaveDataToFileAsync()
        {
            DataProvider.SaveDataToFileAsync(WritingFile, Data).Wait();
            
            using (StreamReader file = new StreamReader(WritingFile))
            {
                foreach (Vertex v in Data)
                {
                    Assert.AreEqual(file.ReadLine(), String.Format("{0} {1}", v.Coordinates.x, v.Coordinates.y));
                }
            }
        }

        [TestMethod]
        public void TestReadFromFileAsync()
        {
            var result = DataProvider.ReadFromFileAsync(ReadingFile);
            result.Wait();
            List<Vertex> dataFromFile = result.Result;

            for (int i = 0; i < 100; i++)
            {
                Point p = dataFromFile[i].Coordinates;
                Assert.AreEqual(Data[i].Coordinates.x, p.x);
                Assert.AreEqual(Data[i].Coordinates.y, p.y);
            }
        }

        [TestMethod]
        public void TestSaveResultsToFile()
        {
            DataProvider.SaveResultsToFileAsync(WritingFile, Tree).Wait();

            using (StreamReader file = new StreamReader(WritingFile))
            {
                foreach (Vertex v in Tree)
                {
                    var line = file.ReadLine();
                    string expectedLine = String.Format("{0} {1} {2}", v.Coordinates.x, v.Coordinates.y, v.Neighbours.Count);
                    foreach (var neighbour in v.Neighbours)
                    {
                        expectedLine += String.Format(" {0} {1}", neighbour.Coordinates.x, neighbour.Coordinates.y);
                    }
                    Assert.AreEqual(line, expectedLine);
                }
            }
        }
    }
}
