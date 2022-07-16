
using QuickSockets.Options;
using QuickSockets.Payloads.Internal;

namespace QuickSockets.Communication.Sending;

/// <summary>
/// Orchistrates sending of Data through TCP-IP sockets
/// </summary>
internal class SenderHandler
{
    private EssentialOptions _essentials;

    public SenderHandler(EssentialOptions essentials)
    {
        _essentials = essentials;
    }

    internal async Task<CommunicationPayload> Send(Connection connection, int port, byte[] data)
    {
        var payload = new CommunicationPayload(connection.IP, _essentials.DeviceIdentifier, data);

        using (var sender = new Sender(connection.IP, port))
        {
            CommunicationPayload? result = await sender.SendData(payload.GetFullData());
        }
    }
}
