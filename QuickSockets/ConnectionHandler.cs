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
    public delegate void DataReceivedEvent(int id, byte[] data);
    public event DataReceivedEvent? DataReceived;

    public delegate void DataReceivedUnknownConnectionEvent(string senderIp, byte[] data);
    public event DataReceivedUnknownConnectionEvent? DataReceivedUnknownConnection;

    public delegate void HandshakeReceivedEvent(string identifier, string ip);
    public event HandshakeReceivedEvent? HandshakeReceived;

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

    public async Task<ListenerChangedResult> StartListeningAsync()
    {
        var result = new ListenerChangedResult();

        try
        {
            List<int> ports = await _listeningHandler.Start();

            result.Successful = true;
            result.Ports = ports;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Exception = e;

            return result;
        }
    }

    public async Task<ListenerChangedResult> StopListeningAsync()
    {
        var result = new ListenerChangedResult();

        try
        {
            List<int> ports = await _listeningHandler.Stop();

            result.Successful = true;
            result.Ports = ports;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Exception = e;

            return result;
        }
    }

    public async Task<ConnectionRegistrationResult> RegisterConnectionAsync(RegistrationPayload payload)
    {
        var result = new ConnectionRegistrationResult();

        try
        {
            result.UniqueId = payload.UniqueId;

            if (_connections.Contains(payload.IP) || _connections.Contains(payload.UniqueId))
                throw new Exceptions.ConnectionAlreadyExistsException();

            CommunicationPayload handshakeResult = await _senderHandler.HandshakeAsync(payload.IP);

            _connections.Add(new Connection(payload, DateTime.Now, Status.Responding, handshakeResult.DeviceIdentifier));

            result.Successful = true;
            result.TimeOfConnection = DateTime.Now;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Exception = e;

            return result;
        }
    }

    public async Task<SendDataResult> SendDataAsync(byte[] data, Dictionary<string, string> attributes, int id)
    {
        var result = new SendDataResult();

        try
        {
            if (!_connections.Contains(id))
                throw new Exceptions.NoExistingConnectionFoundException();

            Connection connection = _connections.Single(c => c.UniqueId == id);

            CommunicationPayload sendResult = await _senderHandler.SendAsync(connection, data, attributes);

            result.UniqueId = id;
            result.TimeOfConnection = DateTime.Now;
            result.Successful = true;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Exception = e;

            return result;
        }
    }

    private void OnDataReceived(CommunicationPayload payload)
    {
        Connection? receivingConnection = _connections.SingleOrDefault(c => c.DeviceIdentifier == payload.DeviceIdentifier);

        if (payload.Type == Enums.PayloadTypes.Data)
        {
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
        else if (payload.Type == Enums.PayloadTypes.Handshake)
        {
            HandshakeReceived?.Invoke(payload.DeviceIdentifier, payload.SenderIp);
        }

    }
}