
namespace QuickSockets.Payloads.Internal;

internal class CommunicationPayload
{
    // Sender Information
    public string SenderIp { get; set; }
    public string DeviceIdentifier { get; set; }
    public byte[] Data { get; set; }
    public DateTime CreationTime { get; set; }

    public CommunicationPayload(string ip, string deviceIdentifier, byte[] data)
    {
        SenderIp = ip;
        DeviceIdentifier = deviceIdentifier;
        Data = data;
        CreationTime = DateTime.Now;
    }
}
