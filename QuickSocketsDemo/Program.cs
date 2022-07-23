using System;
using System.Text;

namespace QuickSocketsDemo;

internal class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("Starting QuickSockets Demo . . .");
        bool running = true;

        while (running)
        {
            Console.WriteLine("Option: HS | SEND | REC | STOP | KILL");
            string i = Console.ReadLine() ?? String.Empty;

            if (i == "SEND")
                Send();
            else if (i == "HS")
                Handshake();
            else if (i == "REC")
                Listen();
            else if (i == "STOP")
                Stop();
            else if (i == "KILL")
                running = false;
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
        _handler.BytesReceived += HandlerBytesReceived;
        _handler.BytesReceivedUnknownConnection += HandlerBytesReceivedUnknownConnection;

        QuickSockets.Results.ListenerChangedResult? result = await _handler.StartListeningAsync();

        if (result != null && result.Ports != null)
            Console.WriteLine($"Listening on Ports: {Spread(result.Ports)} . . .");
        else
            Console.WriteLine("Failed Listening: result == null");
    }

    private static void HandlerBytesReceivedUnknownConnection(int received, int totalReceived, int total, string ip)
    {
        Console.WriteLine($"UNKNOWN: {totalReceived}/{total}");
    }

    private static void HandlerBytesReceived(int received, int totalReceived, int total, int uniqueId)
    {
        Console.WriteLine($"KNOWN: {totalReceived}/{total}");
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

        string filePath = @"C:\Users\bened\Desktop\video.mp4";
        byte[] fileBytes = File.ReadAllBytes(filePath);

        //Console.WriteLine("INPUT DATA");
        //string data = Console.ReadLine() ?? "Default";

        QuickSockets.Results.SendDataResult? result = await _handler.SendDataAsync(fileBytes, null, 1);

        if (result == null)
        {
            Console.WriteLine("NO RESULT");
        }
        else if (result.Exception != null)
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

        if (result == null)
            Console.WriteLine("FAILED: result == null");
        else
        {
            Console.WriteLine(result.UniqueId);
            Console.WriteLine(result.Successful);
        }
    }
}