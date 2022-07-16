using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Options;

public class EssentialOptions
{
    public string OwnIP { get; set; }
    /// <summary>
    /// String that identifies this device. Must be unique within the network to not cause errors
    /// </summary>
    public string DeviceIdentifier { get; set; }
    public PortOptions PortOptions { get; set; }
    public ConnectionValidationOptions ConnectionValidationOptions { get; set; }

    public EssentialOptions(string ownIP, string identifier, PortOptions portOptions, ConnectionValidationOptions connectionValidationOptions)
    {
        OwnIP = ownIP;
        DeviceIdentifier = identifier;
        PortOptions = portOptions;
        ConnectionValidationOptions = connectionValidationOptions;
    }
}
