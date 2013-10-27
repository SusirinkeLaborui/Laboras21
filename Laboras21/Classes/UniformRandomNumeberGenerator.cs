using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    class UniformRandomNumeberGenerator : IRandomNumberGenerator
    {
        private Random random;

        public UniformRandomNumeberGenerator()
        {
            random = new Random();
        }

        public Point GeneratePoint()
        {
            var x = random.Next(-MagicalNumbers.CoordWidth / 2, MagicalNumbers.CoordWidth / 2);
            var y = random.Next(-MagicalNumbers.CoordWidth / 2, MagicalNumbers.CoordWidth / 2);
            return new Point(x, y);
        }
    }
}
