using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21
{
    class BadFileFormatException : Exception
    {
        public BadFileFormatException(string line)
            : base("Incorrect line: " + line)
        {
        }

        public BadFileFormatException(string line, string aditionalError)
            : base("Incorrect line: " + line + ". Error: " + aditionalError)
        {
        }
    }
}
