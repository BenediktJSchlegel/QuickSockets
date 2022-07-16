using System;

namespace QuickSocketsDemo;

internal class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("Starting QuickSockets Demo . . .");

        ConnectSocket();

        Console.WriteLine("Stopping QuickSockets Demo . . .");
        Console.ReadLine();
    }

    private static async void ConnectSocket()
    {
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

        var essentials = new QuickSockets.Options.EssentialOptions("192.168.0.60", "uniqueId", portOptions, connectionValidationOptions);
        var handler = new QuickSockets.ConnectionHandler(essentials);

        var payload = new QuickSockets.Payloads.RegistrationPayload()
        {
            IP = "192.168.0.60",
            UniqueId = 1
        };

        await handler.RegisterConnectionAsync(payload);
    }
}