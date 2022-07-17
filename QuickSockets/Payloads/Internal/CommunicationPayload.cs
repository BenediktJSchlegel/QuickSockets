
using Newtonsoft.Json;
using QuickSockets.Enums;
using System.Text;

namespace QuickSockets.Payloads.Internal;

internal class CommunicationPayload
{
    // Sender Information
    public string SenderIp { get; set; }
    public string DeviceIdentifier { get; set; }
    public byte[] Data { get; set; }
    public DateTime CreationTime { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public PayloadTypes Type { get; set; }

    public CommunicationPayload(PayloadTypes type, string ip, string deviceIdentifier, byte[] data, Dictionary<string, string> attributes)
    {
        this.SenderIp = ip;
        this.DeviceIdentifier = deviceIdentifier;
        this.Data = data;
        this.CreationTime = DateTime.Now;
        this.Attributes = attributes;
        this.Type = type;
    }

    internal byte[] GetBytes()
    {
        string requestAsJson = JsonConvert.SerializeObject(this);

        return Encoding.ASCII.GetBytes(requestAsJson);
    }

    internal byte[] GetFullData()
    {
        var header = new PayloadHeader(this);

        var headerBytes = header.GetBytes();
        var contentBytes = this.GetBytes();

        byte[] fullData = new byte[contentBytes.Length + headerBytes.Length];

        System.Buffer.BlockCopy(headerBytes, 0, fullData, 0, headerBytes.Length);
        System.Buffer.BlockCopy(contentBytes, 0, fullData, headerBytes.Length, contentBytes.Length);

        return fullData;
    }
}
