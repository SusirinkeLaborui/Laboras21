using System;
using System.Runtime.InteropServices;
namespace Laboras21
{
    /// <summary>
    /// Describes one point on Cartesian coordinate system: coordinates x and y.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IComparable<Point>
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        int IComparable<Point>.CompareTo(Point other)
        {
            if (x == other.x)
            {
                return y.CompareTo(other.y);
            }
            else
            {
                return x.CompareTo(other.x);
            }
        }

        // For debugging purposes
        public override string ToString()
        {
            return "(" + x.ToString() + "; " + y.ToString() + ")";
        }
    }
}
