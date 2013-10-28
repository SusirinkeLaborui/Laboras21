using Laboras21.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    public static class DataProvider
    {
        public static async Task<List<Point>> ReadFromFile(string filename)
        {
            var file = new StreamReader(filename);
            var fileContents = await file.ReadToEndAsync();
            var lines = fileContents.Split('\n');
            var data = new List<Point>();

            foreach (var line in lines)
            {
                string[] point = line.Split(' ');
                if (point.Length != 2)
                {
                    throw new BadFileFormatException(line);
                }

                try
                {
                    int x = Convert.ToInt32(point[0]);
                    int y = Convert.ToInt32(point[1]);

                    data.Add(new Point(x, y));
                }
                catch (Exception e)
                {                    
                    throw new BadFileFormatException(line, e.Message);
                }             
            }
            
            return data;
        }

        public static List<Point> Generate(IRandomNumberGenerator rng, int verticesCount)
        {
            List<Point> generatedData = new List<Point>();
            for (int i = 0; i < verticesCount; i++)
            {
                generatedData.Add(rng.GeneratePoint());
            }
            return generatedData;
        }

        /*
         * 12. Rezultatų tekstinis failas bus sudarytas viršūnių kaimynių formatu:
         * kiekviena viršūnei bus skirta viena eilutė
         * ten bus nurodyta viršūnės koordinatės, kaimynių skaičius ir jų koordinatės.
         * Vienos rezultatų eilutės duomenys bus atskiriami tarpu.
         * 
         */
        public static async void SaveToFile(string filename, List<Vertex> vertices)
        {
            using (var file = new StreamWriter(filename))
            {
                var fileContent = new StringBuilder();
                var fileWriteTask = file.WriteAsync(string.Empty);

                foreach (var vertex in vertices)
                {
                    fileContent.AppendFormat("{0} {1} {2}", vertex.Coordinates.x, vertex.Coordinates.y, vertex.Neighbours.Count);

                    foreach (var neighbour in vertex.Neighbours)
                    {
                        fileContent.Append(" {0} {1}", neighbour.Coordinates.x, neighbour.Coordinates.y);

                    }
                    fileContent.Append("\n");

                    await fileWriteTask;
                    fileWriteTask = file.WriteAsync(fileContent.ToString());
                    fileContent.Clear();
                }

                await fileWriteTask;
                file.Close();
            }
        }
    }
}
