﻿using System;
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

        public NormalRandomNumberGenerator(double standardDeviation)
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
                x = random.NextDouble() * 2 - 1;
                y = random.NextDouble() * 2 - 1;
                r = x * x + y * y;
            } while (r == 0.0 || r > 1.0);

            double d = Math.Sqrt(-2.0 * Math.Log(r) / r);
            x *= d * standardDeviation * (MagicalNumbers.MaxX - MagicalNumbers.MinX) * 0.5 + MagicalNumbers.MinX + MagicalNumbers.MaxX;
            y *= d * standardDeviation * (MagicalNumbers.MaxY - MagicalNumbers.MinY) * 0.5 + MagicalNumbers.MinY + MagicalNumbers.MaxY;

            return new Point((int)x, (int)y);
        }
    }
}
