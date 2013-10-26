using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    class NormalRandomNumberGenerator : IRandomNumberGenerator
    {
        private double standardDeviation;
        private Random random;
        NormalRandomNumberGenerator(double standardDeviation)
        {
            this.standardDeviation = standardDeviation;
            random = new Random();
        }

        public Point GeneratePoint()
        {
            // Box-Muller transform.
            // Not sure how it works.
            // http://en.wikipedia.org/wiki/Box-Muller_transform
            double x, y, r;
            do
            {
                x = random.Next(-1, 1);
                y = random.Next(-1, 1);
                r = x * x + y * y;
            } while (r == 0.0 || r > 1.0);
            double d = Math.Sqrt(-2.0 * Math.Log(r) / r);
            x *= d * standardDeviation * 10000;
            y *= d * standardDeviation * 10000;

            return new Point((int)x, (int)y);
        }
    }
}
