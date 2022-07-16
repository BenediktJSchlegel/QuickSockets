
using QuickSockets.Payloads.Internal;

namespace QuickSockets.Socket;

internal class Listener
{
    internal delegate void DataReceivedEvent(CommunicationPayload payload);
    internal event DataReceivedEvent? DataReceived;

    internal void Run()
    {
        throw new NotImplementedException();
    }
}
