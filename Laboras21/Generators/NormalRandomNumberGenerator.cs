using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21.Generators
{
    class NormalRandomNumberGenerator : IRandomNumberGenerator
    {
        private double standardDeviation;
        private Random random;
        private int minX, maxX, minY, maxY;
        private int offsetX, offsetY;
        private int widthX, widthY;
        public NormalRandomNumberGenerator(double standardDeviation, int minX, int maxX, int minY, int maxY)
        {
            this.standardDeviation = standardDeviation;
            random = new Random();
            offsetX = minX + maxX;
            offsetY = minY + maxY;
            widthX = maxX - minX;
            widthY = maxY - minY;
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        public Point GeneratePoint()
        {
            // Box-Muller transform.
            // Not sure how it works.
            // http://en.wikipedia.org/wiki/Box-Muller_transform
            double x, y, r;
            do
            {
                do
                {
                    x = random.NextDouble() * 2 - 1;
                    y = random.NextDouble() * 2 - 1;
                    r = x * x + y * y;
                } while (r == 0.0 || r > 1.0);

                double d = Math.Sqrt(-2.0 * Math.Log(r) / r);

                x *= d * standardDeviation + offsetX;
                y *= d * standardDeviation + offsetY;

            } while (x > maxX || x < minY || y > maxY || y < minY);

            return new Point((int)x, (int)y);
        }

    }
}
