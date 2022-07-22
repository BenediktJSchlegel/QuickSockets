using System;
using System.Text;

namespace QuickSocketsDemo;

internal class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("Starting QuickSockets Demo . . .");

        while (true)
        {
            Console.WriteLine("Option: HS | SEND | REC | STOP");
            string i = Console.ReadLine();

            if (i == "SEND")
                Send();
            else if (i == "HS")
                Handshake();
            else if (i == "REC")
                Listen();
            else if (i == "STOP")
                Stop();
        }


        Console.WriteLine("Stopping QuickSockets Demo . . .");
        Console.ReadLine();
    }

    private static async void Listen()
    {
        Console.WriteLine("STARTING LISTEN . . .");

        var portOptions = new QuickSockets.Options.PortOptions()
        {
            DataPort = 12655,
            HandshakePort = 12656,
            PingPort = 12657
        };

        var connectionValidationOptions = new QuickSockets.Options.ConnectionValidationOptions()
        {
            ContinuouslyPingConnection = true,
            PingInterval = 5
        };

        var essentials = new QuickSockets.Options.EssentialOptions("192.168.0.60", "1", portOptions, connectionValidationOptions);
        _handler = new QuickSockets.ConnectionHandler(essentials);
        
        _handler.DataReceived += HandlerDataReceived;
        _handler.DataReceivedUnknownConnection += HandlerDataReceivedUnknownConnection;

        QuickSockets.Results.ListenerChangedResult? result = await _handler.StartListeningAsync();

        if (result != null && result.Ports != null)
            Console.WriteLine($"Listening on Ports: {Spread(result.Ports)} . . .");
        else
            Console.WriteLine("Failed Listening: result == null");
    }

    private static void HandlerDataReceivedUnknownConnection(string senderIp, byte[] data)
    {
        Console.WriteLine($"DATA RECEIVED UNKNOWN CONNECTION: From IP:{senderIp} Bytes:{data.Length}");
    }

    private static void HandlerDataReceived(int id, byte[] data)
    {
        Console.WriteLine($"DATA RECEIVED: From ID:{id} Bytes:{data.Length}");
    }

    private static string Spread(List<int> list)
    {
        string res = String.Empty;

        foreach (int i in list)
            res += i + ", ";

        return res;
    }

    private static QuickSockets.ConnectionHandler _handler;

    private static async void Stop()
    {
        Console.WriteLine("STOPPING LISTENING");
        
        QuickSockets.Results.ListenerChangedResult? result = await _handler.StopListeningAsync();

        if (result != null && result.Ports != null)
            Console.WriteLine($"Stopped on Ports: {Spread(result.Ports)} . . .");
        else
            Console.WriteLine("Failed Stopping: result == null");
    }

    private static async void Send()
    {
        if (_handler == null)
            throw new ArgumentException();

        Console.WriteLine("INPUT DATA");
        string data = Console.ReadLine();

        QuickSockets.Results.SendDataResult? result = await _handler.SendDataAsync(Encoding.ASCII.GetBytes(data), null, 1);

        if (result == null)
            Console.WriteLine("NO RESULT");
        else if(result != null && result.Exception != null)
        {
            Console.WriteLine(result.Exception);
        }
        else
        {
            Console.WriteLine(result.TimeOfConnection.ToString());
            Console.WriteLine(result.UniqueId.ToString());
        }
    }

    private static async void Handshake()
    {
        Console.WriteLine("STARTING HANDSHAKE . . .");

        var portOptions = new QuickSockets.Options.PortOptions()
        {
            DataPort = 12655,
            HandshakePort = 12656,
            PingPort = 12657
        };

        var connectionValidationOptions = new QuickSockets.Options.ConnectionValidationOptions()
        {
            ContinuouslyPingConnection = true,
            PingInterval = 5
        };

        var essentials = new QuickSockets.Options.EssentialOptions("192.168.0.60", "2", portOptions, connectionValidationOptions);
        _handler = new QuickSockets.ConnectionHandler(essentials);

        var payload = new QuickSockets.Payloads.External.RegistrationPayload()
        {
            IP = "192.168.0.60",
            UniqueId = 1
        };

        QuickSockets.Results.ConnectionRegistrationResult? result = await _handler.RegisterConnectionAsync(payload);

        if(result == null)
            Console.WriteLine("FAILED: result == null");
        else
        {
            Console.WriteLine(result.UniqueId);
            Console.WriteLine(result.Successful);
        }
    }
}