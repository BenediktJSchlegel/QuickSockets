
namespace QuickSockets.Results;

public class ListenerChangedResult
{
    public bool Successful { get; set; }
    public List<int>? Ports { get; set; }
    public Exception? Exception { get; set; }
}
