using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using QuickSockets.Payloads.Internal;
using QuickSockets.Communication;
using QuickSockets.Constants;
using QuickSockets.Exceptions;

internal class Sender : IDisposable
{
    // ManualResetEvent instances signal completion.  
    private static ManualResetEvent _connectDone = new ManualResetEvent(false);
    private static ManualResetEvent _sendDone = new ManualResetEvent(false);
    private static ManualResetEvent _receiveDone = new ManualResetEvent(false);

    private string _response = String.Empty;

    private string _ip;
    private int _port;

    private Socket? _socket;
    private Exception? _thrownException;
    private bool _failedSend = false;

    internal Sender(string ip, int port)
    {
        _ip = ip;
        _port = port;
        _socket = null;
    }

    private void CloseSocket()
    {
        try
        {
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }
        catch (Exception)
        {

        }

        _connectDone.Reset();
        _sendDone.Reset();
        _receiveDone.Reset();
    }

    internal Task<CommunicationPayload> SendData(byte[] data)
    {
        IPAddress ipAddress = IPAddress.Parse(_ip);
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);

        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            _socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), _socket);
            _connectDone.WaitOne();

            if (_failedSend && _thrownException != null)
                throw new FailedSendException("Failed Connecting.", _thrownException);

            Send(_socket, data);

            _sendDone.WaitOne();

            if (_failedSend && _thrownException != null)
                throw new FailedSendException("Failed Sending.", _thrownException);

            Receive(_socket);

            _receiveDone.WaitOne();

            if (_failedSend && _thrownException != null)
                throw new FailedSendException("Failed Receiving Response.", _thrownException);

            CommunicationPayload? response = JsonConvert.DeserializeObject<CommunicationPayload>(_response);

            if (response == null)
                throw new WrongResponseException(_response);

            return Task.FromResult(response);
        }
        finally
        {
            CloseSocket();
        }
    }

    private void ConnectCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is Socket)
            {
                Socket socket = (Socket)asyncResult.AsyncState;

                socket.EndConnect(asyncResult);

                _connectDone.Set();
            }
        }
        catch (Exception ex)
        {
            CloseSocket();
            SetSendingFailed(ex);
        }
    }

    private void Receive(Socket socket)
    {
        try
        {
            var state = new StateObject();
            state.Socket = socket;

            socket.BeginReceive(state.Buffer, 0, ConfigurationConstants.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception ex)
        {
            CloseSocket();
            SetSendingFailed(ex);
        }
    }

    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is StateObject state && state.Socket != null)
            {
                Socket client = state.Socket;

                int bytesRead = client.EndReceive(asyncResult);
                state.ReadDataBytes += bytesRead;

                if (bytesRead > 0)
                {
                    state.DataBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    client.BeginReceive(state.Buffer, 0, ConfigurationConstants.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.DataBuilder.Length > 0)
                    {
                        _response = state.DataBuilder.ToString();
                    }

                    _receiveDone.Set();
                }
            }
        }
        catch (Exception ex)
        {
            CloseSocket();
            SetSendingFailed(ex);
        }
    }

    private void Send(Socket socket, byte[] data)
    {
        try
        {
            socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), socket);
        }
        catch (Exception ex)
        {
            CloseSocket();
            SetSendingFailed(ex);
        }
    }

    private void SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (asyncResult.AsyncState != null && asyncResult.AsyncState is Socket socket)
            {
                socket.EndSend(asyncResult);

                _sendDone.Set();
            }
        }
        catch (Exception ex)
        {
            CloseSocket();
            SetSendingFailed(ex);
        }
    }

    private void SetSendingFailed(Exception ex)
    {
        _failedSend = true;
        _thrownException = ex;

        _sendDone.Set();
        _receiveDone.Set();
        _connectDone.Set();
    }

    public void Dispose()
    {
        try
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }
        catch (Exception)
        {

        }
    }
}