using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    class Vertex
    {
        public Point Coordinates { get; set; }
        public List<Vertex> Neighbours { get; private set; }

        public Vertex(Point coords)
        {
            Coordinates = coords;
            Neighbours = new List<Vertex>();
        }

    }
}
