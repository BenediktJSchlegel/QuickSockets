using Newtonsoft.Json;
using QuickSockets.Constants;
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
    public string ContentType { get; set; }

    public PayloadHeader()
    {
        ContentLength = 0;
        Checksum = String.Empty;
        ContentType = String.Empty;
    }

    public PayloadHeader(CommunicationPayload data)
    {
        //TODO: Checksum
        //TODO: ContentType

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
