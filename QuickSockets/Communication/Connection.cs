﻿using QuickSockets.Enums;
using QuickSockets.Payloads.External;

namespace QuickSockets.Communication;

internal class Connection
{
    public int UniqueId { get; set; }
    public string IP { get; set; }
    public string DeviceIdentifier { get; set; }
    public DateTime LastContact { get; set; }
    public Status Status { get; set; }

    public Connection(RegistrationPayload payload)
    {
        this.UniqueId = payload.UniqueId;
        this.IP = payload.IP;

        //TODO:
        this.LastContact = DateTime.Now;
        this.Status = Status.NotResponding;
        this.DeviceIdentifier = String.Empty;
    }
}