
namespace QuickSockets.Results;

public abstract class CommunicationResult
{
    public int UniqueId { get; set; }
    public DateTime TimeOfConnection { get; set; }
    public bool Successful { get; set; }
    public Exception? Exception { get; set; }
}
