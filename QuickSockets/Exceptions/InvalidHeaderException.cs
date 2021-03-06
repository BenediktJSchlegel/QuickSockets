using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Exceptions
{
    internal class InvalidHeaderException : Exception
    {
        public string HeaderValue { get; }

        public InvalidHeaderException(string headerValue)
        {
            HeaderValue = headerValue;
        }
    }
}
