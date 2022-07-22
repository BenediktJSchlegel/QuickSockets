using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Exceptions
{
    public class FailedSendException : Exception
    {
        public FailedSendException(string? message, Exception? exception) : base(message, exception)
        {

        }
    }
}
