
using QuickSockets.Enums;

namespace QuickSockets.Results;

public class ConnectionRegistrationResult
{
    public int UniqueId { get; set; }
    public DateTime TimeOfConnection { get; set; }
    public bool Successful { get; set; }
}
