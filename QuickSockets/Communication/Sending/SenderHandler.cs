
using QuickSockets.Options;

namespace QuickSockets.Communication.Sending;

/// <summary>
/// Orchistrates all TCP-Senders and their threads
/// </summary>
internal class SenderHandler
{
    private EssentialOptions _essentials;

    public SenderHandler(EssentialOptions essentials)
    {
        _essentials = essentials;
    }

    internal void Que(Connection connection, byte[] data)
    {
        throw new NotImplementedException();
    }
}
