using QuickSockets.Constants;
using QuickSockets.Payloads.Internal;
using System.Net.Sockets;
using System.Text;

namespace QuickSockets.Communication;

internal class StateObject
{
    internal byte[] Buffer { get; set; }

    internal byte[] HeaderBuffer { get; set; }

    internal int ReadDataBytes { get; set; }

    internal StringBuilder DataBuilder { get; set; } = new StringBuilder();

    internal System.Net.Sockets.Socket? Socket { get; set; } = null;

    internal bool HeaderHasBeenRead => ReceivedHeader != null;

    internal PayloadHeader? ReceivedHeader { get; set; }

    internal string? LocalEndpointIP { get; set; }

    internal StateObject()
    {
        Buffer = new byte[ConfigurationConstants.BUFFER_SIZE];
        HeaderBuffer = new byte[ConfigurationConstants.HEADER_SIZE];
    }
}
