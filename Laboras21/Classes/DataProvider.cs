﻿using Laboras21.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Laboras21
{
    public static class DataProvider
    {
        public static async Task<List<Vertex>> ReadFromFileAsync(string filename)
        {

            using (var file = new StreamReader(filename))
            {
                var fileContents = await file.ReadToEndAsync();
                var lines = fileContents.Split('\n');
                var data = new List<Vertex>();
                if (lines.Length - 1 > MagicalNumbers.MaxN)
                {
                    throw new BadFileFormatException("File is too long");
                }

                for (var i = 0; i < lines.Length - 1; i++)
                {
                    string[] point = lines[i].Split(' ');
                    if (point.Length != 2)
                    {
                        throw new BadFileFormatException(lines[i]);
                    }

                    try
                    {
                        int x = Convert.ToInt32(point[0]);
                        int y = Convert.ToInt32(point[1]);
                        if (x > MagicalNumbers.MaxX || x < MagicalNumbers.MinX || y > MagicalNumbers.MaxY || y < MagicalNumbers.MinY)
                        {
                            throw new BadFileFormatException(lines[i]);
                        }
                        data.Add(new Vertex(new Point(x, y)));
                    }
                    catch (Exception e)
                    {
                        throw new BadFileFormatException(lines[i], e.Message);
                    };
                }
                return data;
            }
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
