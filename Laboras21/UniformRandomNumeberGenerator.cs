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
            var x = random.Next(-10000, 10000);
            var y = random.Next(-10000, 10000);
            return new Point(x, y);
        }
    }
}
