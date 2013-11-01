using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21.Generators
{
    class UniformRandomNumberGenerator : IRandomNumberGenerator
    {
        private Random random;
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;
        public UniformRandomNumberGenerator(int minX, int maxX, int minY, int maxY)
        {
            random = new Random();
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        public Point GeneratePoint()
        {
            var x = random.Next(minX, maxX);
            var y = random.Next(minY, maxY);
            return new Point(x, y);
        }
    }
}
