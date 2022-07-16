
using QuickSockets.Options;
using QuickSockets.Payloads.Internal;
using QuickSockets.Socket;

namespace QuickSockets.Communication.Listening;

/// <summary>
/// Orchistrates all TCP-Listeners and the threads associated with them
/// </summary>
internal class ListeningHandler
{
    internal delegate void DataReceivedEvent(CommunicationPayload payload);
    internal event DataReceivedEvent? DataReceived;

    private List<KeyValuePair<Thread, Listener>> _workers;

    private EssentialOptions _essentials;

    public ListeningHandler(EssentialOptions essentials)
    {
        _workers = new List<KeyValuePair<Thread, Listener>>();
        _essentials = essentials;
    }

    /// <summary>
    ///  Dummy for testing
    /// </summary>
    private void SpawnListener(object options)
    {
        var listener = new Listener();
        var thread = new Thread(() =>
        {
            listener.DataReceived += OnDataReceived;
            listener.Run();
        });

        _workers.Add(new KeyValuePair<Thread, Listener>(thread, listener));

        thread.Start();

    }

    private void OnDataReceived(Payloads.Internal.CommunicationPayload payload)
    {
        DataReceived?.Invoke(payload);
    }
}
