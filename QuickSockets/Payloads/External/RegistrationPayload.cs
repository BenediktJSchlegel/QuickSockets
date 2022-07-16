using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Payloads.External
{
    public class RegistrationPayload
    {
        public int UniqueId { get; set; }
        public string IP { get; set; }
    }
}
