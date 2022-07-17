using QuickSockets.Options;
using QuickSockets.Payloads.Internal;

namespace QuickSockets.Communication.Listening;

/// <summary>
/// Orchistrates all TCP-Listeners and the threads associated with them
/// </summary>
internal class ListeningHandler
{
    internal delegate void DataReceivedEvent(CommunicationPayload payload);
    internal event DataReceivedEvent? DataReceived;

    private EssentialOptions _essentials;
    private List<KeyValuePair<Thread, Listener>> _workers;

    internal ListeningHandler(EssentialOptions essentials)
    {
        _essentials = essentials;
        _workers = new List<KeyValuePair<Thread, Listener>>();
    }

    internal Task<List<int>> Start()
    {
        var ports = new List<int>()
        {
            _essentials.PortOptions.HandshakePort,
            _essentials.PortOptions.PingPort,
            _essentials.PortOptions.DataPort
        }.Distinct().ToList();

        foreach (int port in ports)
        {
            KeyValuePair<Thread, Listener> worker = SpawnListener(_essentials.OwnIP, port);

            worker.Value.DataReceived += OnDataReceived;
            worker.Key.Start();

            _workers.Add(worker);
        }

        return Task.FromResult(ports);
    }

    internal Task<bool> Stop()
    {
        try
        {
            foreach (KeyValuePair<Thread, Listener> worker in _workers)
            {
                worker.Value.Stop();
            }

            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }

    private KeyValuePair<Thread, Listener> SpawnListener(string ip, int port)
    {
        var listener = new Listener(ip, port, _essentials);
        var thread = new Thread(() =>
        {
            listener.Run();
        });

        return new KeyValuePair<Thread, Listener>(thread, listener);
    }

    private void OnDataReceived(Payloads.Internal.CommunicationPayload payload)
    {
        if (payload.Type == Enums.PayloadTypes.Data)
            DataReceived?.Invoke(payload);
    }
}
