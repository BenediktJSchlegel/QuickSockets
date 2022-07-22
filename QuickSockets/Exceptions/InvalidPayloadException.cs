using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Exceptions
{
    public class InvalidPayloadException : Exception
    {
        public string ReceivedData { get; }

        public InvalidPayloadException(string data)
        {
            this.ReceivedData = data;
        }
    }
}
