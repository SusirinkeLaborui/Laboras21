using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    class UniformRandomNumberGenerator : IRandomNumberGenerator
    {
        private Random random;

        public UniformRandomNumberGenerator()
        {
            random = new Random();
        }

        public Point GeneratePoint()
        {
            var x = random.Next(MagicalNumbers.MinX, MagicalNumbers.MaxX);
            var y = random.Next(MagicalNumbers.MinY, MagicalNumbers.MaxY);
            return new Point(x, y);
        }
    }
}
