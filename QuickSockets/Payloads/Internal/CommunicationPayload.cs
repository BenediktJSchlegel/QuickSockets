
using Newtonsoft.Json;
using QuickSockets.Enums;
using System.Text;

namespace QuickSockets.Payloads.Internal;

internal class CommunicationPayload
{
    // Sender Information
    public string SenderIp { get; set; }
    public string DeviceIdentifier { get; set; }
    [JsonIgnore]
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

    private byte[] GetDataBytes()
    {
        byte[] dividerBytes = Encoding.ASCII.GetBytes(Constants.ConfigurationConstants.DATA_DIVIDER);
        var fullDataBytes = new byte[dividerBytes.Length + this.Data.Length];

        System.Buffer.BlockCopy(dividerBytes, 0, fullDataBytes, 0, dividerBytes.Length);
        System.Buffer.BlockCopy(this.Data, 0, fullDataBytes, dividerBytes.Length, this.Data.Length);

        return fullDataBytes;
    }

    internal byte[] GetFullData()
    {
        var header = new PayloadHeader(this);

        byte[] headerBytes = header.GetBytes();
        byte[] payloadBytes = this.GetBytes();
        byte[] dataBytes = this.GetDataBytes();

        byte[] fullData = new byte[payloadBytes.Length + headerBytes.Length + dataBytes.Length];

        System.Buffer.BlockCopy(headerBytes, 0, fullData, 0, headerBytes.Length);
        System.Buffer.BlockCopy(payloadBytes, 0, fullData, headerBytes.Length, payloadBytes.Length);
        System.Buffer.BlockCopy(dataBytes, 0, fullData, headerBytes.Length + payloadBytes.Length, dataBytes.Length);

        return fullData;
    }
}
