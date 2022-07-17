
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

    internal async Task<CommunicationPayload> SendAsync(Connection connection, byte[] data, Dictionary<string, string> attributes)
    {
        var payload = new CommunicationPayload(Enums.PayloadTypes.Data, _essentials.OwnIP, _essentials.DeviceIdentifier, data, attributes);

        return await SendPayloadAsync(connection.IP, _essentials.PortOptions.DataPort, payload);
    }

    internal async Task<CommunicationPayload> HandshakeAsync(string ip)
    {
        var payload = new CommunicationPayload(Enums.PayloadTypes.Handshake, _essentials.OwnIP, _essentials.DeviceIdentifier, new byte[0], new Dictionary<string, string>());

        return await SendPayloadAsync(ip, _essentials.PortOptions.HandshakePort, payload);
    }

    private async Task<CommunicationPayload> SendPayloadAsync(string ip, int port, CommunicationPayload payload)
    {
        using (var sender = new Sender(ip, port))
        {
            CommunicationPayload? result = await sender.SendData(payload.GetFullData());

            return result;
        }
    }
}
