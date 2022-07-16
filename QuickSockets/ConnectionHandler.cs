using QuickSockets.Communication;
using QuickSockets.Communication.Listening;
using QuickSockets.Communication.Sending;
using QuickSockets.Enums;
using QuickSockets.Options;
using QuickSockets.Payloads.External;
using QuickSockets.Payloads.Internal;
using QuickSockets.Results;
using QuickSockets.Validation;

namespace QuickSockets;

public class ConnectionHandler
{
    internal delegate void DataReceivedEvent(int id, byte[] data);
    internal event DataReceivedEvent? DataReceived;

    internal delegate void DataReceivedUnknownConnectionEvent(string senderIp, byte[] data);
    internal event DataReceivedUnknownConnectionEvent? DataReceivedUnknownConnection;

    private EssentialOptions _essentials;
    private ConnectionList _connections;

    private ListeningHandler _listeningHandler;
    private SenderHandler _senderHandler;

    public ConnectionHandler(EssentialOptions essentials)
    {
        if (!OptionValidation.EssentialOptionsAreValid(_essentials))
            throw new Exception("Options are not valid");

        _essentials = essentials;
        _connections = new ConnectionList();

        _listeningHandler = new ListeningHandler(_essentials);
        _senderHandler = new SenderHandler(_essentials);

        _listeningHandler.DataReceived += OnDataReceived;
    }

    private void OnDataReceived(CommunicationPayload payload)
    {
        Connection? receivingConnection = _connections.SingleOrDefault(c => c.DeviceIdentifier == payload.DeviceIdentifier);

        if (receivingConnection != null)
        {
            receivingConnection.LastContact = DateTime.Now;
            receivingConnection.Status = Status.Responding;

            DataReceived?.Invoke(receivingConnection.UniqueId, payload.Data);
        }
        else
        {
            DataReceivedUnknownConnection?.Invoke(payload.SenderIp, payload.Data);
        }
    }

    public async Task<ConnectionRegistrationResult> RegisterConnectionAsync(RegistrationPayload payload)
    {
        if (_connections.Contains(payload.IP) || _connections.Contains(payload.UniqueId))
            throw new Exceptions.ConnectionAlreadyExistsException();

        throw new NotImplementedException();
    }

    public async Task<object> SendDataAsync(byte[] data, int id)
    {
        if (!_connections.Contains(id))
            throw new Exceptions.NoExistingConnectionFoundException();

        throw new NotImplementedException();
    }

    public async Task<object> SendDataAsync(byte[] data, string ip)
    {
        if (!_connections.Contains(ip))
            throw new Exceptions.NoExistingConnectionFoundException();

        throw new NotImplementedException();
    }

}