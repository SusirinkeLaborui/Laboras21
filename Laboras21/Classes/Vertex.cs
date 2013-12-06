using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    /// <summary>
    /// Describes one graph vertex - its coordinates and list of its neighbours.
    /// </summary>
    public class Vertex
    {
        public Point Coordinates { get; set; }
        public List<Vertex> Neighbours { get; private set; }

        public Vertex(Point coords)
        {
            Coordinates = coords;
            Neighbours = new List<Vertex>();
        }

        // For debugging purposes
        public override string ToString()
        {
            return "(" + Coordinates.x.ToString() + "; " + Coordinates.y.ToString() + ")";
        }
    }
}
