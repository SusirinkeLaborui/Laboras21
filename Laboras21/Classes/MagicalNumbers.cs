using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    /// <summary>
    /// Describes global program constants.
    /// </summary>
    static class MagicalNumbers
    {
        public const int MinX = -500;
        public const int MaxX = 500;
        public const int MinY = -500;
        public const int MaxY = 500;
        public const int DataWidth = MaxX - MinX;
        public const int DataHeight = MaxY - MinY;

        public const int MaxN = 10002;

        public const int DefaultStandardDeviation = 125;
    }
}
