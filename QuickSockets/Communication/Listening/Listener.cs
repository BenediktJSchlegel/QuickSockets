
using Newtonsoft.Json;
using QuickSockets.Communication;
using QuickSockets.Constants;
using QuickSockets.Payloads.Internal;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using QuickSockets.Exceptions;

namespace QuickSockets.Communication.Listening;

internal class Listener
{
    internal delegate void DataReceivedEvent(CommunicationPayload payload);
    internal event DataReceivedEvent? DataReceived;

    internal delegate void ExceptionRaisedEvent(Exception ex);
    internal event ExceptionRaisedEvent? ExceptionRaised;

    private string _ip;
    private int _port;
    private bool _running = false;

    private Socket? _listeningSocket;
    private Options.EssentialOptions _essentials;

    private ManualResetEvent _threadReset = new ManualResetEvent(false);

    public int Port => _port;

    internal Listener(string ip, int port, Options.EssentialOptions essentials)
    {
        _ip = ip;
        _port = port;
        _essentials = essentials;
    }

    internal void Run()
    {
        IPAddress ipAddress = IPAddress.Parse(_ip);
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

        _listeningSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _running = true;

        try
        {
            _listeningSocket.Bind(localEndPoint);
            _listeningSocket.Listen(100);

            while (_running)
            {
                try
                {
                    Console.WriteLine($"Starting Listener on Port {_port}");

                    _threadReset.Reset();
                    _listeningSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listeningSocket);
                    _threadReset.WaitOne();
                }
                catch (Exception)
                {
                    // Error while Sending or Receiving.
                    // Close Socket? Maybe Socket does not get property closed?
                    ResetSocket();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    internal void Stop()
    {
        _running = false;

        ResetSocket();
    }

    private void ResetSocket()
    {
        try
        {
            if (_listeningSocket != null)
            {
                _listeningSocket.Close();
                _listeningSocket.Shutdown(SocketShutdown.Both);
            }
        }
        catch (Exception)
        {
        }

        _threadReset.Set();
    }

    internal void AcceptCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is Socket listener)
            {
                _threadReset.Set();

                Socket handler = listener.EndAccept(asyncResult);

                // Create the state object.  
                StateObject state = new StateObject();
                state.Socket = handler;

                // First Receive. Read the header to the HeaderBuffer
                handler.BeginReceive(state.HeaderBuffer, 0, ConfigurationConstants.HEADER_SIZE, 0, new AsyncCallback(ReadCallback), state);
            }
        }
        catch (Exception)
        {
            ResetSocket();
        }
    }

    internal void ReadCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is StateObject state && state.Socket != null)
            {
                Socket socket = state.Socket;

                int bytesRead = socket.EndReceive(asyncResult);

                if (bytesRead > 0)
                {
                    if (state.HeaderHasBeenRead && state.ReceivedHeader != null)
                    {
                        // Read actual data
                        state.DataBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                        state.ReadDataBytes += bytesRead;

                        if (state.ReadDataBytes < state.ReceivedHeader.ContentLength)
                        {
                            socket.BeginReceive(state.Buffer, 0, ConfigurationConstants.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
                        }
                        else
                        {
                            CommunicationPayload? payload = JsonConvert.DeserializeObject<CommunicationPayload>(state.DataBuilder.ToString());

                            if (payload != null)
                            {
                                DataReceived?.Invoke(payload);

                                var response = new CommunicationPayload(Enums.PayloadTypes.Confirmation, _essentials.OwnIP, _essentials.DeviceIdentifier, Array.Empty<byte>(), new Dictionary<string, string>());
                                string responseAsJson = JsonConvert.SerializeObject(response);

                                Send(socket, responseAsJson);
                            }
                            else
                            {
                                throw new InvalidPayloadException(state.DataBuilder.ToString());
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(bytesRead == ConfigurationConstants.HEADER_SIZE);

                        // First Read => Read the Header
                        string headerData = Encoding.ASCII.GetString(state.HeaderBuffer, 0, bytesRead);
                        PayloadHeader? header = JsonConvert.DeserializeObject<PayloadHeader>(headerData);

                        if (header == null)
                            throw new InvalidHeaderException(headerData);

                        state.ReceivedHeader = header;

                        // Header was received => Read the Data
                        socket.BeginReceive(state.Buffer, 0, ConfigurationConstants.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ExceptionRaised?.Invoke(ex);
            ResetSocket();
        }

    }

    private void Send(Socket socket, string data)
    {
        try
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }
        catch (Exception)
        {
            ResetSocket();
        }
    }

    private void SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is Socket socket)
            {
                // Complete sending the data to the remote device.  
                int bytesSent = socket.EndSend(asyncResult);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        catch (Exception)
        {
            ResetSocket();
        }
    }

    public void Dispose()
    {
        try
        {
            if (_listeningSocket != null)
            {
                _listeningSocket.Shutdown(SocketShutdown.Both);
                _listeningSocket.Close();
            }
        }
        catch (Exception)
        {

        }
    }
}

