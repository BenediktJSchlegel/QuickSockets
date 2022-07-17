using Newtonsoft.Json;
using QuickSockets.Constants;
using QuickSockets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Payloads.Internal;

internal class PayloadHeader
{
    public int ContentLength { get; set; }
    public string Checksum { get; set; }
    public PayloadTypes Type { get; set; }

    public PayloadHeader()
    {
        this.ContentLength = 0;
        this.Checksum = String.Empty;
    }

    public PayloadHeader(CommunicationPayload data)
    {
        //TODO: Checksum
        this.Checksum = String.Empty;
        this.Type = data.Type;
        this.ContentLength = data.GetBytes().Length;
    }

    public byte[] GetBytes()
    {
        var payload = new byte[ConfigurationConstants.HEADER_SIZE];

        string objectAsJson = JsonConvert.SerializeObject(this);
        byte[] dataBytes = Encoding.ASCII.GetBytes(objectAsJson);

        Array.Copy(dataBytes, 0, payload, 0, dataBytes.Length);

        return payload;
    }
}
