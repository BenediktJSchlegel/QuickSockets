namespace QuickSockets.Options;

public class PortOptions
{
    /// <summary>
    /// The Port used to establish initial contact with a device.
    /// Only basic information necessary for further communication will be exchanged.
    /// </summary>
    public int HandshakePort { get; set; }
    /// <summary>
    /// The Port used to Ping a device to check if it is (still) responsive and listening
    /// </summary>
    public int PingPort { get; set; }
    /// <summary>
    /// The Port used to transfer actual Data
    /// </summary>
    public int DataPort { get; set; }
}
