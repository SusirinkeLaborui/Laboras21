using System;
namespace Laboras21
{
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
                return x.CompareTo(other.y);
            }
        }
    }
}
